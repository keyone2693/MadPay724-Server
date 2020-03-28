using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Common.Routes.V1.Site;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.V1.Panel.Accountant
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class BankCardsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BankCardsController> _logger;



        public BankCardsController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper,
            ILogger<BankCardsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(SiteV1Routes.Accountant.GetBankCards)]
        public async Task<IActionResult> GetBankCards([FromQuery]PaginationDto paginationDto)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
                   .GetAllPagedListAsync(
                   paginationDto,
                   paginationDto.Filter.ToBankCardExpression(true),
                   paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                   "");

            Response.AddPagination(bankCardsFromRepo.CurrentPage, bankCardsFromRepo.PageSize,
                bankCardsFromRepo.TotalCount, bankCardsFromRepo.TotalPage);

            var bankCards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);


            return Ok(bankCards);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(SiteV1Routes.Accountant.GetInventoryBankCard)]
        public async Task<IActionResult> GetUserBankCards(string userId)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
           .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");


            var bankcards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);

            return Ok(bankcards);
        }
    }
}
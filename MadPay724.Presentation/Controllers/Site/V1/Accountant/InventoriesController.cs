using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoriesController> _logger;



        public InventoriesController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper,
            ILogger<InventoriesController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventories)]
        public async Task<IActionResult> GetInventories(string id, [FromQuery]PaginationDto paginationDto)
        {

            var usersFromRepo = await _db.UserRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToUserExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "Wallets,Photos");//,BankCards

            Response.AddPagination(usersFromRepo.CurrentPage, usersFromRepo.PageSize,
                usersFromRepo.TotalCount, usersFromRepo.TotalPage);

            var users = new List<UserForAccountantDto>();

            foreach (var item in usersFromRepo)
            {
                users.Add(_mapper.Map<UserForAccountantDto>(item));
            }

            return Ok(users);
        }




    }
}
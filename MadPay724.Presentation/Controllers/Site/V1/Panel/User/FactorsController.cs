using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Factors;
using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Panel.User
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class FactorsController : ControllerBase
    {
        private readonly IUnitOfWork<Financial_MadPayDbContext> _db;
        private readonly IUnitOfWork<Main_MadPayDbContext> _dbMain;
        private readonly IMapper _mapper;
        private readonly ILogger<FactorsController> _logger;


        public FactorsController(IUnitOfWork<Financial_MadPayDbContext> db, IMapper mapper,
            ILogger<FactorsController> logger,IUnitOfWork<Main_MadPayDbContext> dbMain)
        {
            _dbMain = dbMain;
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.UsersFactors.GetGateFactors)]
        public async Task<IActionResult> GetGateFactors(string userId, string gateId,
    [FromQuery]FactorPaginationDto paginationDto)
        {

            var factorsFromRepo = await _db.FactorRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.ToFactorExpression(SearchIdEnums.Gate, gateId, userId, false),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");

            Response.AddPagination(factorsFromRepo.CurrentPage, factorsFromRepo.PageSize,
                factorsFromRepo.TotalCount, factorsFromRepo.TotalPage);

            return Ok(factorsFromRepo);
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet(ApiV1Routes.UsersFactors.GetFactor)]
        public async Task<IActionResult> GetFactor(string userId, string factorId)
        {
            var factorFromRepo = await _db.FactorRepository.GetByIdAsync(factorId);
            if (factorFromRepo != null)
            {
                if(factorFromRepo.UserId != userId)
                {
                    return BadRequest("امکان دسترسی به فاکتور کاربر دیگیری را ندارید");
                }
                var userFromRepo = await _dbMain.UserRepository.GetByIdAsync(factorFromRepo.UserId);
                factorFromRepo.UserName = userFromRepo.Name;
                _db.FactorRepository.Update(factorFromRepo);
                await _db.SaveAsync();
                //
                var gatefromRepo = await _dbMain.GateRepository.GetByIdAsync(factorFromRepo.GateId);
                //
                var walletfromRepo = await _dbMain.WalletRepository.GetByIdAsync(factorFromRepo.EnterMoneyWalletId);

                var model = new FactorForReturnDto
                {
                    Factor = factorFromRepo,
                    Gate = _mapper.Map<GateForReturnDto>(gatefromRepo),
                    Wallet = _mapper.Map<WalletForReturnDto>(walletfromRepo),
                };

                return Ok(model);
            }
            else
            {
                return BadRequest("فاکتور وجود ندارد");
            }

        }
    }
}
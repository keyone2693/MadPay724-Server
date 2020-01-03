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
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Wallet.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class FactorsController : ControllerBase
    {
        private readonly IUnitOfWork<Financial_MadPayDbContext> _db;
        private readonly IUnitOfWork<Main_MadPayDbContext> _dbMain;
        private readonly IMapper _mapper;
        private readonly ILogger<FactorsController> _logger;
        private readonly IWalletService _walletService;


        public FactorsController(IUnitOfWork<Financial_MadPayDbContext> db, IMapper mapper,
            ILogger<FactorsController> logger, IWalletService walletService, IUnitOfWork<Main_MadPayDbContext> dbMain)
        {
            _dbMain = dbMain;
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _walletService = walletService;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Factors.GetFactors)]
        public async Task<IActionResult> GetFactors([FromQuery]PaginationDto paginationDto)
        {

            var factorsFromRepo = await _db.FactorRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToFactorExpression(SearchIdEnums.None),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Factors

            Response.AddPagination(factorsFromRepo.CurrentPage, factorsFromRepo.PageSize,
                factorsFromRepo.TotalCount, factorsFromRepo.TotalPage);

            return Ok(factorsFromRepo);
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Factors.GetWalletFactors)]
        public async Task<IActionResult> GetWalletFactors(string walletId, [FromQuery]PaginationDto paginationDto)
        {

            var factorsFromRepo = await _db.FactorRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToFactorExpression(SearchIdEnums.Wallet, walletId),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Factors

            Response.AddPagination(factorsFromRepo.CurrentPage, factorsFromRepo.PageSize,
                factorsFromRepo.TotalCount, factorsFromRepo.TotalPage);

            return Ok(factorsFromRepo);
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Factors.GetFactor, Name = "GetFactor")]
        public async Task<IActionResult> GetFactor(string factorId)
        {
            var factorFromRepo = await _db.FactorRepository.GetByIdAsync(factorId);
            if (factorFromRepo != null)
            {
                var userFromRepo = await _dbMain.UserRepository.GetByIdAsync(factorFromRepo.UserId);

                factorFromRepo.UserName = userFromRepo.Name;

                _db.FactorRepository.Update(factorFromRepo);
                await _db.SaveAsync();

                return Ok(factorFromRepo);
            }
            else
            {
                return BadRequest("فاکتور وجود ندارد");
            }

        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Factors.StatusFactor)]
        public async Task<IActionResult> StatusFactor(string factorId, ChangeStatusFactorDto changeStatusFactorDto)
        {
            var factorFromRepo = await _db.FactorRepository.GetByIdAsync(factorId);
            if (factorFromRepo != null)
            {
                //تایید فاکتور
                if (changeStatusFactorDto.Status)
                {
                    if (factorFromRepo.Status)
                    {
                        return NoContent();
                    }
                    else
                    {
                        var incInventory = await _walletService
                            .IncreaseInventoryAsync(factorFromRepo.EndPrice, factorFromRepo.EnterMoneyWalletId, true);

                        if (!incInventory.status)
                        {
                            return BadRequest("خطا در اپدیت اطلاعات کیف پول کاربر");
                        }
                    }
                }
                //عدم تایید فاکتور
                else
                {
                    if (!factorFromRepo.Status)
                    {
                        return NoContent();
                    }
                    else
                    {
                        var decInventory = await _walletService
                            .DecreaseInventoryAsync(factorFromRepo.EndPrice, factorFromRepo.EnterMoneyWalletId, true);
                        if (!decInventory.status)
                        {
                            return BadRequest("خطا در اپدیت اطلاعات کیف پول کاربر");
                        }
                    }
                }
                factorFromRepo.Status = changeStatusFactorDto.Status;
                if (await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطا در تغییر تاییدی  فاکتور");
                }
            }
            else
            {
                return BadRequest("فاکتور با این شناسه یافت نشد");
            }
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Factors.EditFactor)]
        public async Task<IActionResult> EditFactor(string factorId, EditFactorDto editFactorDto)
        {
            var factorFromRepo = await _db.FactorRepository.GetByIdAsync(factorId);
            if (factorFromRepo != null)
            {
                factorFromRepo.RefBank = editFactorDto.RefBank;
                if (await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطا در ویرایش فاکتور");
                }
            }
            else
            {
                return BadRequest("فاکتور با این شناسه یافت نشد");
            }
        }
        [Authorize(Policy = "RequireNoAccess")]
        [HttpDelete(ApiV1Routes.Factors.DeleteFactor)]
        public async Task<IActionResult> DeleteFactor(string factorId)
        {
            var factorFromRepo = await _db.FactorRepository.GetByIdAsync(factorId);
            if (factorFromRepo != null)
            {
                _db.FactorRepository.Delete(factorFromRepo);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در حذف اطلاعات");
            }
            else
            {
                return BadRequest("فاکتور وجود ندارد");
            }
        }
    }
}
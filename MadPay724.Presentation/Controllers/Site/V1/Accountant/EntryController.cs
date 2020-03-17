using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Entry;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IUnitOfWork<Financial_MadPayDbContext> _db;
        private readonly IUnitOfWork<Main_MadPayDbContext> _dbMain;
        private readonly IMapper _mapper;
        private readonly ILogger<EntryController> _logger;
        private readonly IWalletService _walletService;
        public EntryController(
            IUnitOfWork<Financial_MadPayDbContext> dbContext,
            IUnitOfWork<Main_MadPayDbContext> dbMain,
            IMapper mapper,
            ILogger<EntryController> logger, IWalletService walletService)
        {
            _db = dbContext;
            _dbMain = dbMain;
            _mapper = mapper;
            _logger = logger;
            _walletService = walletService;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetApproveEntries)]
        public async Task<IActionResult> GetApproveEntries([FromQuery]PaginationDto paginationDto)
        {
            var entriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(EntryState.Approve, SearchIdEnums.None),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Entries

            Response.AddPagination(entriesFromRepo.CurrentPage, entriesFromRepo.PageSize,
                entriesFromRepo.TotalCount, entriesFromRepo.TotalPage);

            return Ok(entriesFromRepo);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetPardakhtEntries)]
        public async Task<IActionResult> GetPardakhtEntries([FromQuery]PaginationDto paginationDto)
        {

            var entriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(EntryState.Pardakht, SearchIdEnums.None),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Entries

            Response.AddPagination(entriesFromRepo.CurrentPage, entriesFromRepo.PageSize,
                entriesFromRepo.TotalCount, entriesFromRepo.TotalPage);

            return Ok(entriesFromRepo);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetDoneEntries)]
        public async Task<IActionResult> GetDoneEntries([FromQuery]PaginationDto paginationDto)
        {

            var entriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(EntryState.Archive,SearchIdEnums.None),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Entries

            Response.AddPagination(entriesFromRepo.CurrentPage, entriesFromRepo.PageSize,
                entriesFromRepo.TotalCount, entriesFromRepo.TotalPage);

            return Ok(entriesFromRepo);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetBankCardEntries)]
        public async Task<IActionResult> GetBankCardEntries(string bankcardId, [FromQuery]PaginationDto paginationDto)
        {

            var bancardEntriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(EntryState.All, SearchIdEnums.BankCard, bankcardId),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");

            Response.AddPagination(bancardEntriesFromRepo.CurrentPage, bancardEntriesFromRepo.PageSize,
                bancardEntriesFromRepo.TotalCount, bancardEntriesFromRepo.TotalPage);

            return Ok(bancardEntriesFromRepo);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetWalletEntries)]
        public async Task<IActionResult> GetWalletEntries(string walletId, [FromQuery]PaginationDto paginationDto)
        {
            var walletEntriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(EntryState.All, SearchIdEnums.Wallet, walletId),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");

            Response.AddPagination(walletEntriesFromRepo.CurrentPage, walletEntriesFromRepo.PageSize,
                walletEntriesFromRepo.TotalCount, walletEntriesFromRepo.TotalPage);

            return Ok(walletEntriesFromRepo);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Entry.GetEntry, Name = "GetEntry")]
        public async Task<IActionResult> GetEntry(string entryId)
        {

            var entryFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);

            var bankCardFromRepo = await _dbMain.BankCardRepository.GetByIdAsync(entryFromRepo.BankCardId);

            var walletFromRepo = await _dbMain.WalletRepository.GetByIdAsync(entryFromRepo.WalletId);

            entryFromRepo.BankName = bankCardFromRepo.BankName;
            entryFromRepo.OwnerName = bankCardFromRepo.OwnerName;
            entryFromRepo.Shaba = bankCardFromRepo.Shaba;
            entryFromRepo.CardNumber = bankCardFromRepo.CardNumber;
            entryFromRepo.WalletName = walletFromRepo.Name;

            _db.EntryRepository.Update(entryFromRepo);

            await _db.SaveAsync();

            if (entryFromRepo != null)
            {
                return Ok(entryFromRepo);
            }
            else
            {
                return BadRequest("واریزی وجود ندارد");
            }

        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Entry.UpdateEntry)]
        public async Task<IActionResult> UpdateEntry(string entryId, EntryForUpdateDto entryForUpdateDto)
        {
            var entyFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);

            if (entyFromRepo != null)
            {
                entyFromRepo.TextForUser = entryForUpdateDto.TextForUser;
                entyFromRepo.BankTrackingCode = entryForUpdateDto.BankTrackingCode;

                _db.EntryRepository.Update(entyFromRepo);

                if (await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطا در ثبت پیغام برای کاربر");
                }
            }
            else
            {
                return BadRequest("واریزی با این شناسه یافت نشد");
            }
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Entry.ApproveEntry)]
        public async Task<IActionResult> ApproveEntry(string entryId, ChangeEntryStateDto changeEntryStateDto)
        {
            var entyFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);
            if (entyFromRepo != null)
            {
                if (!entyFromRepo.IsPardakht && !entyFromRepo.IsReject)
                {
                    entyFromRepo.IsApprove = changeEntryStateDto.IsApprove;
                    if (await _db.SaveAsync())
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest("خطا در تغییر تاییدی امکان واریز");
                    }
                }
                else
                {
                    return BadRequest("امکان تایید واریزی برای واریزی های رد شده یا پرداخت شده وجود ندارد");
                }
            }
            else
            {
                return BadRequest("واریزی با این شناسه یافت نشد");
            }
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Entry.PardakhtEntry)]
        public async Task<IActionResult> PardakhtEntry(string entryId, ChangeEntryStateDto changeEntryStateDto)
        {
            var entyFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);
            if (entyFromRepo != null)
            {
                if (entyFromRepo.IsApprove && !entyFromRepo.IsReject)
                {
                    //کاهش موجودی
                    if (changeEntryStateDto.IsPardakht)
                    {
                        if (entyFromRepo.IsPardakht)
                        {
                            return NoContent();
                        }
                        else
                        {
                            var decreaseInventoryResult = await _walletService
                                .EntryDecreaseInventoryAsync(entyFromRepo.Price, entyFromRepo.WalletId);

                            if (!decreaseInventoryResult.status)
                            {
                                return BadRequest("خطا در تغغیر موجودی کیف پول احتمالا کیف ول موجود یندارد");
                            }
                        }
                    }
                    //افزایش موجودی
                    else
                    {
                        if (!entyFromRepo.IsPardakht)
                        {
                            return NoContent();
                        }
                        else
                        {
                            var decreaseInventoryResult = await _walletService
                                .EntryIncreaseInventoryAsync(entyFromRepo.Price, entyFromRepo.WalletId);

                            if (!decreaseInventoryResult.status)
                            {
                                return BadRequest("خطا در تغغیر موجودی کیف پول احتمالا کیف ول موجود یندارد");
                            }
                        }
                    }
                    entyFromRepo.IsPardakht = changeEntryStateDto.IsPardakht;
                    if (await _db.SaveAsync())
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest("خطا در تغییر تاییدی پرداخت واریز");
                    }
                }
                else
                {
                    return BadRequest("امکان پرداخت واریزی برای واریزی های رد شده یا تایید نشده وجود ندارد");
                }
            }
            else
            {
                return BadRequest("واریزی با این شناسه یافت نشد");
            }
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Entry.RejectEntry)]
        public async Task<IActionResult> RejectEntry(string entryId, ChangeEntryStateDto changeEntryStateDto)
        {
            var entyFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);
            if (entyFromRepo != null)
            {
                if (!entyFromRepo.IsPardakht)
                {
                    //کاهش پول خروجی
                    if (changeEntryStateDto.IsReject)
                    {
                        if (entyFromRepo.IsReject)
                        {
                            return NoContent();
                        }
                        else
                        {
                            var decreaseOnExitMoneyResult = await _walletService
                                .EntryDecreaseOnExitMoneyAsync(entyFromRepo.Price, entyFromRepo.WalletId);

                            if (!decreaseOnExitMoneyResult.status)
                            {
                                return BadRequest("خطا در تغیر پول خروجی کیف پول ");
                            }
                        }
                    }
                    //افزایش پول خروجی
                    else
                    {
                        if (!entyFromRepo.IsReject)
                        {
                            return NoContent();
                        }
                        else
                        {
                            var decreaseOnExitMoneyResult = await _walletService
                                .EntryIncreaseOnExitMoneyAsync(entyFromRepo.Price, entyFromRepo.WalletId);

                            if (!decreaseOnExitMoneyResult.status)
                            {
                                return BadRequest("خطا در تغیر پول خروجی کیف پول ");
                            }
                        }
                    }
                    entyFromRepo.IsReject = changeEntryStateDto.IsReject;
                    if (await _db.SaveAsync())
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest("خطا در تغییر رد درخواست واریز");
                    }
                }
                else
                {
                    return BadRequest("امکان رد واریزی برای واریزی های پرداخت شده اند وجود ندارد");
                }
            }
            else
            {
                return BadRequest("واریزی با این شناسه یافت نشد");
            }
        }
        [Authorize(Policy = "RequireNoAccess")]
        [HttpDelete(ApiV1Routes.Entry.DeleteEntry)]
        public async Task<IActionResult> DeleteEntry(string entryId)
        {
            var entryFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);
            if (entryFromRepo != null)
            {
                _db.EntryRepository.Delete(entryFromRepo);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در حذف اطلاعات");
            }
            else
            {
                return BadRequest("واریزی وجود ندارد");
            }
        }
    }
}
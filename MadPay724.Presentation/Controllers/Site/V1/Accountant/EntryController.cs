using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Entry;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class EntryController : ControllerBase
    {

        private readonly IUnitOfWork<Financial_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<EntryController> _logger;

        public EntryController(IUnitOfWork<Financial_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<EntryController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "AccessAccounting")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Entry.GetEntries)]
        public async Task<IActionResult> GetEntries([FromQuery]PaginationDto paginationDto)
        {

            var entriesFromRepo = await _db.EntryRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToEntryExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");//,Entries

            Response.AddPagination(entriesFromRepo.CurrentPage, entriesFromRepo.PageSize,
                entriesFromRepo.TotalCount, entriesFromRepo.TotalPage);

            return Ok(entriesFromRepo);
        }

        [Authorize(Policy = "AccessAccounting")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Entry.GetEntry, Name = "GetEntry")]
        public async Task<IActionResult> GetEntry(string entryId)
        {
            var entryFromRepo = await _db.EntryRepository.GetByIdAsync(entryId);
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
        [Authorize(Policy = "AccessAccounting")]
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
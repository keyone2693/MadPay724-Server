
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Ticket;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketsController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;

        public TicketsController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<TicketsController> logger, IUploadService uploadService,
            IWebHostEnvironment env)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminTicket.GetTickets)]
        public async Task<IActionResult> GetTickets([FromQuery]TicketsPaginationDto ticketsPaginationDto)
        {
            var ticketsFromRepo = await _db.TicketRepository
               .GetAllPagedListAsync(
               ticketsPaginationDto,
               ticketsPaginationDto.ToTicketExpression(SearchIdEnums.None),
               ticketsPaginationDto.SortHe.ToOrderBy(ticketsPaginationDto.SortDir),
               "");//,tickets

            Response.AddPagination(ticketsFromRepo.CurrentPage, ticketsFromRepo.PageSize,
                ticketsFromRepo.TotalCount, ticketsFromRepo.TotalPage);

            return Ok(ticketsFromRepo);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminTicket.GetTicket)]
        public async Task<IActionResult> GetTicket(string ticketId)
        {
            var ticketFromRepo = (await _db.TicketRepository.GetManyAsync(p => p.Id == ticketId, null, "TicketContents"))
                .SingleOrDefault();
            if (ticketFromRepo != null)
            {
                ticketFromRepo.TicketContents = ticketFromRepo.TicketContents.OrderBy(p => p.DateCreated).ToList();
                return Ok(ticketFromRepo);
            }
            else
            {
                return BadRequest("تیکتی وجود ندارد");
            }
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPatch(ApiV1Routes.AdminTicket.SetTicketClosed)]
        public async Task<IActionResult> SetTicketClosed(string ticketId, UpdateTicketClosed updateTicketClosed)
        {
            var ticketFromRepo = (await _db.TicketRepository.GetByIdAsync(ticketId));
            if (ticketFromRepo != null)
            {
                ticketFromRepo.Closed = updateTicketClosed.Closed;
                _db.TicketRepository.Update(ticketFromRepo);
                if (await _db.SaveAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("خطا در ثبت اطلاعات ");
                }
            }
            else
            {
                return BadRequest("تیکتی وجود ندارد");
            }

        }

        //--------------------------------------------------------------------------------------------------------------------------------
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminTicket.GetTicketContent, Name = "GetAdminTicketContent")]
        public async Task<IActionResult> GetAdminTicketContent(string ticketContentId)
        {
            var ticketContentFromRepo = await _db.TicketContentRepository.GetByIdAsync(ticketContentId);
            if (ticketContentFromRepo != null)
            {
                return Ok(ticketContentFromRepo);
            }
            else
            {
                return BadRequest("تیکتی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminTicket.GetTicketContents)]
        public async Task<IActionResult> GetTicketContents(string ticketId)
        {
            var ticketFromRepo = await _db.TicketContentRepository.GetManyAsync(p => p.TicketId == ticketId,
                s => s.OrderByDescending(x => x.DateCreated), "");
            return Ok(ticketFromRepo);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost(ApiV1Routes.AdminTicket.AddTicketContent)]
        public async Task<IActionResult> AddTicketContent(string ticketId, [FromForm]TicketContentForCreateDto ticketContentForCreateDto)
        {
            var ticketContent = new TicketContent()
            {
                TicketId = ticketId,
                IsAdminSide = true,
                Text = ticketContentForCreateDto.Text
            };
            if (ticketContentForCreateDto.File != null)
            {
                if (ticketContentForCreateDto.File.Length > 0)
                {
                    var uploadRes = await _uploadService.UploadFileToLocal(
                        ticketContentForCreateDto.File,
                        Path.GetRandomFileName(),
                        _env.WebRootPath,
                        $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                        "Files\\TicketContent\\"+ DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day
                    );
                    if (uploadRes.Status)
                    {
                        ticketContent.FileUrl = uploadRes.Url;
                    }
                    else
                    {
                        return BadRequest(uploadRes.Message);
                    }
                }
                else
                {
                    ticketContent.FileUrl = "";
                }
            }
            else
            {
                ticketContent.FileUrl = "";
            }


            await _db.TicketContentRepository.InsertAsync(ticketContent);

            if (await _db.SaveAsync())
            {
                return CreatedAtRoute("GetAdminTicketContent", new { ticketContentId = ticketContent.Id }, ticketContent);
            }
            else
            {
                return BadRequest("خطا در ثبت اطلاعات ");

            }


        }
    }
}
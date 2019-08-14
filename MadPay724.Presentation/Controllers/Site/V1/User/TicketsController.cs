
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Ticket;
using MadPay724.Data.Models;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketsController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;

        public TicketsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<TicketsController> logger, IUploadService uploadService,
            IWebHostEnvironment env)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Ticket.GetTickets)]
        public async Task<IActionResult> GetTickets(string userId)
        {
            var ticketsFromRepo = await _db.TicketRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderBy(x => x.Closed).ThenByDescending(x => x.DateModified), "");

            // var tickets = _mapper.Map<List<TicketForReturnDto>>(ticketsFromRepo);

            return Ok(ticketsFromRepo);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Ticket.GetTicket, Name = "GetTicket")]
        public async Task<IActionResult> GetTicket(string id, string userId)
        {
            var ticketFromRepo = await _db.TicketRepository.GetByIdAsync(id);
            if (ticketFromRepo != null)
            {
                if (ticketFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    return Ok(ticketFromRepo);
                }
                else
                {
                    _logger.LogError($"کاربر   {userId} قصد دسترسی به تیکت دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به تیکت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("تیکتی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Ticket.AddTicket)]
        public async Task<IActionResult> AddTicket(string userId, TicketForCreateDto ticketForCreateDto)
        {
            var ticketFromRepo = await _db.TicketRepository
                .GetAsync(p => p.Title == ticketForCreateDto.Title && p.UserId == userId);

            if (ticketFromRepo == null)
            {
                var ticket = new Ticket()
                {
                    UserId = userId,
                    Closed = false,
                    IsAdminSide = false
                };

                _mapper.Map(ticketForCreateDto, ticket);

                await _db.TicketRepository.InsertAsync(ticket);

                if (await _db.SaveAsync())
                {
                    return CreatedAtRoute("GetTicket", new { id = ticket.Id, userId = userId },
                        ticket);
                }
                else
                {
                    return BadRequest("خطا در ثبت اطلاعات ");

                }
            }
            {
                return BadRequest("این تیکت قبلا ثبت شده است");
            }


        }

        //--------------------------------------------------------------------------------------------------------------------------------
        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Ticket.GetTicketContent, Name = "GetTicketContent")]
        public async Task<IActionResult> GetTicketContent(string id, string userId)
        {
            var ticketFromRepo = await _db.TicketContentRepository.GetByIdAsync(id);
            if (ticketFromRepo != null)
            {
                if (ticketFromRepo.Ticket.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    return Ok(ticketFromRepo);
                }
                else
                {
                    _logger.LogError($"کاربر   {userId} قصد دسترسی به تیکت دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به تیکت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("تیکتی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Ticket.GetTicketContents)]
        public async Task<IActionResult> GetTicketContents(string id, string userId)
        {
            var ticketFromRepo = (await _db.TicketRepository.GetManyAsync(p => p.Id == id,
                s => s.OrderByDescending(x => x.DateCreated), "TicketContents")).SingleOrDefault();
            if (ticketFromRepo == null)
                return Ok(new List<TicketContent>());
            else
                return Ok(ticketFromRepo.TicketContents.ToList());
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Ticket.AddTicketContent)]
        public async Task<IActionResult> AddTicketContent(string id, string userId, [FromForm]TicketContentForCreateDto ticketContentForCreateDto)
        {
            var ticketContent = new TicketContent()
            {
                TicketId = id,
                IsAdminSide = false,
                Text = ticketContentForCreateDto.Text
            };
            if (ticketContentForCreateDto.File.Length > 0)
            {
                var uploadRes = await _uploadService.UploadFileToLocal(
                    ticketContentForCreateDto.File,
                    userId,
                    _env.WebRootPath,
                    $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                    "Files\\TicketContent"
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

            await _db.TicketContentRepository.InsertAsync(ticketContent);

            if (await _db.SaveAsync())
            {
                return CreatedAtRoute("GetTicketContent", new { id = ticketContent.Id, userId = userId },
                    ticketContent);
            }
            else
            {
                return BadRequest("خطا در ثبت اطلاعات ");

            }


        }
    }
}
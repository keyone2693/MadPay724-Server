
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
using Microsoft.AspNetCore.Authorization;
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

        public TicketsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<TicketsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
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
                    // var ticket = _mapper.Map<TicketForReturnDto>(ticketFromRepo);

                    return Ok(ticketFromRepo);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به تیکت دیگری را دارد");

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

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Ticket.AddTicketContent)]
        public async Task<IActionResult> AddTicketContent(string userId, TicketContentForCreateDto ticketContentForCreateDto)
        {
            var ticket = new Ticket()
            {
                UserId = userId,
                Closed = false,
                IsAdminSide = false
            };

            _mapper.Map(ticketContentForCreateDto, ticket);

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
    }
}
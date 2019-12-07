
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Notification;
using MadPay724.Data.Models.MainDB;
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
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadpayDbContext> _db;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IMapper _mapper;

        public NotificationsController(IUnitOfWork<Main_MadpayDbContext> dbContext, ILogger<NotificationsController> logger,
            IMapper mapper)
        {
            _db = dbContext;
            _logger = logger;
            _mapper = mapper;
        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(ApiV1Routes.Notification.UpdateUserNotify)]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        public async Task<IActionResult> UpdateUserNotify(string userId, NotificationForUpdateDto notificationForUpdateDto)
        {
            var notifyFromRepo = (await _db.NotificationRepository
                .GetManyAsync(p => p.UserId == userId, null, "")).SingleOrDefault();

            if (notifyFromRepo != null)
            {
                var notifyForUpdate = _mapper.Map(notificationForUpdateDto, notifyFromRepo);

                _db.NotificationRepository.Update(notifyForUpdate);

                if (await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطای ثبت در دیتابیس");
                }
            }
            else
            {
                var notifyToCreate = new Notification
                {
                    UserId = userId
                };
                var notifyForCreate = _mapper.Map(notificationForUpdateDto, notifyToCreate);
                await _db.NotificationRepository.InsertAsync(notifyForCreate);
                if (await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطای ثبت در دیتابیس");
                }
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet(ApiV1Routes.Notification.GetUserNotify)]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        public async Task<IActionResult> GetUserNotify(string userId)
        {
            var notifyFromRepo = (await _db.NotificationRepository
                .GetManyAsync(p => p.UserId == userId, null, "")).SingleOrDefault();
            
            if (notifyFromRepo != null)
            {
                if (notifyFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    return Ok(notifyFromRepo);
                }
                else
                {
                    _logger.LogError($"کاربر   {userId} قصد دسترسی به اطلاعات notify دیگری را دارد");
                    return Unauthorized($"شما اجازه دسترسی به این اطلاعات را ندارید");
                }
            }
            else
            {

                return BadRequest("اطلاعات اطلاع رسانی وجود ندارد");

            }

        }
    }
}
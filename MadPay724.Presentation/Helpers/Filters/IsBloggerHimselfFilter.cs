using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace MadPay724.Presentation.Helpers.Filters
{
    public class IsBloggerHimselfFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAcc;
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        public IsBloggerHimselfFilter(IUnitOfWork<Main_MadPayDbContext> dbContext,
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAcc)
        {
            _db = dbContext;
            _logger = loggerFactory.CreateLogger("IsBloggerHimselfFilter");
            _httpContextAcc = httpContextAcc;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var blogId = context.RouteData.Values["id"].ToString();

            var blogFromRepo =_db.BlogRepository
                .GetMany(p => p.Id == blogId, null, "User").FirstOrDefault();

            if (blogFromRepo != null)
            {
                if (!_httpContextAcc.HttpContext.User.HasClaim(ClaimTypes.Role, "AdminBlog") && !_httpContextAcc.HttpContext.User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    if (blogFromRepo.UserId != _httpContextAcc.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    {
                        _logger.LogError($"کاربر   {context.RouteData.Values["userId"]} قصد دسترسی به بلاگ دیگری را دارد");

                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        base.OnActionExecuting(context);
                    }
                }
                else
                {
                    base.OnActionExecuting(context);
                }
            }
            else
            {
                context.Result = new BadRequestResult();
            }

        }
    }
}

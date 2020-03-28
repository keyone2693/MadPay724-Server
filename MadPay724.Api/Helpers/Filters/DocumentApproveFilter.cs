using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using ActionExecutingContext = Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
using ActionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute;

namespace MadPay724.Api.Helpers.Filters
{
    public class DocumentApproveFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAcc;
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        public DocumentApproveFilter(ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAcc,
            IUnitOfWork<Main_MadPayDbContext> db)
        {
            _logger = loggerFactory.CreateLogger("UserCheckIdFilter");
            _httpContextAcc = httpContextAcc;
            _db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values["id"] != null && context.RouteData.Values["userId"] == null)
            {
                //id = userId
                var result = _db.DocumentRepository.GetMany(p => p.UserId == context.RouteData.Values["id"].ToString()
                                                                 && (p.Approve == 1), null, "");
                if (result.Any())
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                //userId = userId
                var result = _db.DocumentRepository.GetMany(p => p.UserId == context.RouteData.Values["userId"].ToString()
                                                                 && (p.Approve == 1), null, "");
                if (result.Any())
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    context.Result = new ForbidResult();
                }
            }

        }
    }
}

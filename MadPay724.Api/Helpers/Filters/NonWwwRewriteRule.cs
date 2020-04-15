using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

namespace MadPay724.Api.Helpers.Filters
{
    public class NonWwwRewriteRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            if (request.Host.Value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                var response = context.HttpContext.Response;

                string redirectUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
                response.Headers[HeaderNames.Location] = redirectUrl;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Result = RuleResult.EndResponse;
            }
        }
    }
}

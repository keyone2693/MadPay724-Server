using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

namespace MadPay724.Api.Helpers.Filters
{
    public class NonWwwRewriteRule : IRule
    {
        public virtual void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var path = request.Path.ToString().Trim();
           
            var response = context.HttpContext.Response; 
            if (string.IsNullOrEmpty(path))
            {
                path = "swagger";
                if (request.Host.Value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                {
                    string redirectUrl = $"{request.Scheme}://{request.Host.Value.Replace("www.", "")}{new PathString(path)}{request.QueryString}";
                    response.Headers[HeaderNames.Location] = redirectUrl;
                    response.StatusCode = StatusCodes.Status301MovedPermanently;
                    context.Result = RuleResult.EndResponse;
                }
                else
                {
                    string redirectUrl = $"{request.Scheme}://{request.Host}{new PathString(path)}{request.QueryString}";
                    response.Headers[HeaderNames.Location] = redirectUrl;
                    response.StatusCode = StatusCodes.Status301MovedPermanently;
                    context.Result = RuleResult.EndResponse;
                }
            }
            else
            {
                if (request.Host.Value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                {
                    string redirectUrl = $"{request.Scheme}://{request.Host.Value.Replace("www.", "")}{request.Path}{request.QueryString}";
                    response.Headers[HeaderNames.Location] = redirectUrl;
                    response.StatusCode = StatusCodes.Status301MovedPermanently;
                    context.Result = RuleResult.EndResponse;
                }
            }

        }
    }
}

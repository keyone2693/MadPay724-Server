using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;

namespace MadPay724.Payment.Helpers.Filters
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
                string redirectUrl = "https://madpay724.ir";
                response.Headers[HeaderNames.Location] = redirectUrl;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Result = RuleResult.EndResponse;
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

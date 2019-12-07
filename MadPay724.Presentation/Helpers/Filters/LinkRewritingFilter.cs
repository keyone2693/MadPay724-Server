using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.Dtos.Common.ION;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MadPay724.Presentation.Helpers.Filters
{
    public class LinkRewritingFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var asObjResult = context.Result as ObjectResult;
            bool shouldSkip = asObjResult?.Value == null || asObjResult?.StatusCode != (int) HttpStatusCode.OK;
            if (shouldSkip)
            {
                await next();
                return;
            }

            var rewriter = new LinkRewriter(_urlHelperFactory.GetUrlHelper(context));
            ReWriteAllLinks(asObjResult.Value, rewriter);

            await next();
        }

        private static  void ReWriteAllLinks(object model , LinkRewriter rewriter)
        {
            if (model == null) return;



                  var allProperties = model.GetType().GetTypeInfo().GetAllProperties()
                .Where(p => p.CanRead).ToArray();

            var linkProperties = allProperties
                .Where(p => p.CanWrite && p.PropertyType == typeof(Link));

            foreach (var linkPeoperty in linkProperties)
            {
                var rewritten = rewriter.Rewrite(linkPeoperty.GetValue(model) as Link);
                if(rewritten == null) continue;

                linkPeoperty.SetValue(model, rewritten);

                if (linkPeoperty.Name == nameof(BaseDto.Self))
                {
                    allProperties.SingleOrDefault(p=>p.Name == nameof(BaseDto.Href))
                        ?.SetValue(model,rewritten.Href);

                    allProperties.SingleOrDefault(p => p.Name == nameof(BaseDto.Method))
                        ?.SetValue(model, rewritten.Method);

                    allProperties.SingleOrDefault(p => p.Name == nameof(BaseDto.Relations))
                        ?.SetValue(model, rewritten.Relations);
                }

            }

            var arrayProperties = allProperties.Where(P => P.PropertyType.IsArray);
            RewriteLinkInArrays(arrayProperties, model, rewriter);

            var objectProperties = allProperties.Except(linkProperties).Except(arrayProperties);
            RewriterLinksInNestdObjects(objectProperties, model, rewriter);
            }

          

        private static void RewriterLinksInNestdObjects(
            IEnumerable<PropertyInfo> objectProperties,
            object obj,
            LinkRewriter rewriter)
        {
            foreach (var objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                {
                    continue;
                }

                var typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    ReWriteAllLinks(objectProperty.GetValue(obj), rewriter);
                }
            }
        }

        private static void RewriteLinkInArrays(
            IEnumerable<PropertyInfo> arrayProperties,
            object obj,
            LinkRewriter rewriter)
        {
            foreach (var arraProperty in arrayProperties)
            {
                var array = arraProperty.GetValue(obj) as Array ?? new Array[0];

                foreach (var el in array)
                {
                    ReWriteAllLinks(el, rewriter);
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Common.ION;

namespace MadPay724.Common.Helpers.MediaTypes
{
  public  class LinkRewriter
  {
      private readonly IUrlHelper _urlHelper;

      public LinkRewriter(IUrlHelper urlHelper)
      {
          _urlHelper = urlHelper;
      }

      public Link Rewrite(Link original)
      {
          if (original == null) return null;

          return new Link()
          {
              Href = _urlHelper.Link(original.RouteName,original.RouteValues),
              Method = original.Method,
              Relations = original.Relations
          };
      }
  }
}

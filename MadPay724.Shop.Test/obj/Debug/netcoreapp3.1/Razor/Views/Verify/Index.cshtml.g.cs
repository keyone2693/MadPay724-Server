#pragma checksum "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "39ff5a2d26e63389acd2233aa940a3db86fba33e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Verify_Index), @"mvc.1.0.view", @"/Views/Verify/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\_ViewImports.cshtml"
using MadPay724.Shop.Test;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\_ViewImports.cshtml"
using MadPay724.Shop.Test.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
using MadPay724.AspNetCore.GateWay.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"39ff5a2d26e63389acd2233aa940a3db86fba33e", @"/Views/Verify/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d52c27e1ad8f5fe27fb7112368f5993527b151d", @"/Views/_ViewImports.cshtml")]
    public class Views_Verify_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MadPayGateResult<MadPayGateVerifyResponse>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
  
    ViewData["Title"] = "فروشگاه کفش چرم";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    <div class=\"text-center\">\r\n");
#nullable restore
#line 8 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
         if (Model.Status)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <button type=\"button\" class=\"btn btn-success\">پرداخت شما با موفقیت انجام شد</button>\r\n            <br />\r\n            <div class=\"alert alert-info\" role=\"alert\">\r\n");
#nullable restore
#line 13 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                 foreach (var error in Model.Messages)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <span>");
#nullable restore
#line 15 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                     Write(error);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n");
#nullable restore
#line 16 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </div>\r\n            <br />\r\n            <h2>کفش مردانه ی چرم اصل ایرانی <span class=\"badge badge-secondary\">300,000 تومان</span></h2>\r\n");
#nullable restore
#line 20 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"

        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <button type=\"button\" class=\"btn btn-danger\">پرداخت شما موفقیت نبود</button>\r\n            <br />\r\n            <div class=\"alert alert-danger\" role=\"alert\">\r\n");
#nullable restore
#line 27 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                 foreach (var error in Model.Messages)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <span>");
#nullable restore
#line 29 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                     Write(error);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n");
#nullable restore
#line 30 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </div>\r\n            <br />\r\n            <h2>کفش مردانه ی چرم اصل ایرانی <span class=\"badge badge-secondary\">300,000 تومان</span></h2>\r\n");
#nullable restore
#line 34 "D:\Tuts\Daneshjooyar\ProjectFile\MadPay724-Server\MadPay724.Shop.Test\Views\Verify\Index.cshtml"

        }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MadPayGateResult<MadPayGateVerifyResponse>> Html { get; private set; }
    }
}
#pragma warning restore 1591

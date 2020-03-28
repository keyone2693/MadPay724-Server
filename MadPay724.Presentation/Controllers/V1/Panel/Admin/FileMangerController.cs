using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Common.Routes.V1.Site;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;

namespace MadPay724.Presentation.Controllers.V1.Panel.Admin
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    public class FileMangerController : ControllerBase
    {

        public PhysicalFileProvider opration;
        public string basePath;
        private readonly string root = "wwwroot\\Files";


        public FileMangerController(IHostingEnvironment hostingEnvironment)
        {
            basePath = hostingEnvironment.ContentRootPath;
            opration = new PhysicalFileProvider();
            opration.RootFolder(basePath + "\\" + this.root);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost(SiteV1Routes.AdminFileManager.FileOperations)]
        public async Task<IActionResult> FileOperations([FromBody]FileManagerDirectoryContent args)
        {
            //if(args.Action == "delete" || args.Action == "rename")
            //{
            //    if(string.IsNullOrEmpty(args.TargetPath) && string.IsNullOrEmpty(args.Path))
            //    {
            //        FileManagerResponse response = new FileManagerResponse();

            //        response.Error = new ErrorDetails
            //        {
            //            Code = "401",
            //            Message = "عدم دسترسی به فولدر مورد نظر"
            //        };
            //        return Unauthorized(opration.ToCamelCase(response));
            //    }
            //}
            switch (args.Action)
            {
                case "read":
                    return Ok(opration.ToCamelCase(opration.GetFiles(args.Path, args.ShowHiddenItems)));
                case "delete":
                    return Ok(opration.ToCamelCase(opration.Delete(args.Path, args.Names)));
                case "copy":
                    return Ok(opration.ToCamelCase(opration.Copy(args.Path, args.TargetPath,args.Names,args.RenameFiles,args.TargetData)));
                case "move":
                    return Ok(opration.ToCamelCase(opration.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData)));
                case "details":
                    return Ok(opration.ToCamelCase(opration.Details(args.Path, args.Names)));
                case "create":
                    return Ok(opration.ToCamelCase(opration.Create(args.Path, args.Name)));
                case "search":
                    return Ok(opration.ToCamelCase(opration.Search(args.Path, args.SearchString,args.ShowHiddenItems,args.CaseSensitive)));
                case "rename":
                    return Ok(opration.ToCamelCase(opration.Rename(args.Path, args.Name, args.NewName)));

            }
            return Ok();
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost(SiteV1Routes.AdminFileManager.Upload)]

        public async Task<IActionResult> Upload([FromForm]FileManagerDirectoryContent args, IList<IFormFile> uploadFiles)
        {
            //if(false)
            //{
            //    Response.Clear();
            //    Response.ContentType = "application/json; charset-utf8";
            //    Response.StatusCode = 403;
            //    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "";
            //}
            FileManagerResponse uploadResponse = opration.Upload(args.Path, uploadFiles, args.Action, null);
            if (uploadResponse.Error == null)
                return NoContent();
            else
                return BadRequest(uploadResponse.Error.Message);
        }
         [AllowAnonymous]
        [HttpPost(SiteV1Routes.AdminFileManager.Download)]
        public async Task<IActionResult> Download([FromForm]string downloadInput)
        {
            FileManagerDirectoryContent args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);

            return opration.Download(args.Path, args.Names);
        }

        [AllowAnonymous]
        [HttpGet(SiteV1Routes.AdminFileManager.GetImage)]

        public async Task<IActionResult> GetImage([FromQuery]FileManagerDirectoryContent args)
        {
            return opration.GetImage(args.Path, args.Id,true,null,null);
        }
    }
}
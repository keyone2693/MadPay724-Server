using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Presentation.Routes.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
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
        [HttpGet(ApiV1Routes.AdminFileManager.FileOperations)]
        public async Task<IActionResult> FileOperations([FromBody]FileManagerDirectoryContent args)
        {
            if(args.Action == "delete" || args.Action == "rename")
            {
                if(string.IsNullOrEmpty(args.TargetPath) && string.IsNullOrEmpty(args.Path))
                {
                    FileManagerResponse response = new FileManagerResponse();

                    response.Error = new ErrorDetails
                    {
                        Code = "401",
                        Message = "عدم دسترسی به فولدر مورد نظر"
                    };
                    return Unauthorized(opration.ToCamelCase(response));
                }
            }
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

    }
}
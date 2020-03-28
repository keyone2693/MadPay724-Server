using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Utilities;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Models.MainDB.Blog;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Site.Site.Blog;
using MadPay724.Data.Dtos.Site.Panel.BlogGroup;

namespace MadPay724.Presentation.Controllers.Site.V1.Site
{

    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Layout")]
    [ApiController]
    [AllowAnonymous]
    public class LayoutController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<LayoutController> _logger;
        private ApiReturn<string> errorModel;

        public LayoutController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<LayoutController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            errorModel = new ApiReturn<string>
            {
                Status = false,
                Message = "",
                Result = null
            };
        }
        [HttpGet(ApiV1Routes.Layout.GetSidebarData)]
        [ProducesResponseType(typeof(ApiReturn<BlogsReturnDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSidebarData()
        {
            var model = new ApiReturn<BlogsReturnDto>
            {
                Result = new BlogsReturnDto()
            };
            //Blogs

            model.Result.Blogs = null;
            //MostViewed
            var mostVieweBlogsFromRepo = await _db.BlogRepository
             .GetManyAsync(p => p.Status,
             s => s.OrderByDescending(x => x.DateModified), "User,BlogGroup", 5);
            model.Result.MostViewed = (mostVieweBlogsFromRepo.Select(item => _mapper.Map<BlogForReturnDto>(item))).ToList();
            //MostCommented
            model.Result.MostCommented = null;
            //BlogGroups
            var blogGroupFromRepo = await _db.BlogGroupRepository.GetAllAsync();
            model.Result.BlogGroups = _mapper.Map<List<BlogGroupForReturnDto>>(blogGroupFromRepo);

            model.Status = true;
            model.Message = "اطلاعات با موفقیت بارگزاری شد";
            return Ok(model);

        }
    }
}
﻿using System;
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
    [ApiExplorerSettings(GroupName = "v1_Site_Blog")]
    [ApiController]
    [AllowAnonymous]
    public class BlogsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogsController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IUtilities _utilities;
        private ApiReturn<string> errorModel;

        public BlogsController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<BlogsController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IUtilities utilities)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _utilities = utilities;
            errorModel = new ApiReturn<string>
            {
                Status = false,
                Message = "",
                Result = null
            };
        }
        [HttpGet(ApiV1Routes.SiteBlog.GetBlogs)]
        [ProducesResponseType(typeof(ApiReturn<BlogsReturnDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBlogs([FromQuery]PaginationDto paginationDto)
        {
            var model = new ApiReturn<BlogsReturnDto>
            {
                Result = new BlogsReturnDto()
            };
            //Blogs
            var blogsFromRepo = await _db.BlogRepository
                .GetAllPagedListAsync(
                paginationDto,
                paginationDto.Filter.ToBlogExpressionForSite(paginationDto.SortHe, paginationDto.SortDir),
                "DateModified,desc",
                "User,BlogGroup");

            Response.AddPagination(blogsFromRepo.CurrentPage, blogsFromRepo.PageSize,
                blogsFromRepo.TotalCount, blogsFromRepo.TotalPage);
            model.Result.Blogs = (blogsFromRepo.Select(item => _mapper.Map<BlogForReturnDto>(item))).ToList();
            //MostViewed
            var mostVieweBlogsFromRepo = await _db.BlogRepository
             .GetManyAsync(p => p.Status,
             s => s.OrderByDescending(x => x.ViewCount), "User,BlogGroup", 5);
            model.Result.MostViewed = (mostVieweBlogsFromRepo.Select(item => _mapper.Map<BlogForReturnDto>(item))).ToList();
            //MostCommented
            var mostCommentedBlogsFromRepo = await _db.BlogRepository
                .GetManyAsync(p => p.Status,
                s => s.OrderBy(x => x.ViewCount), "User,BlogGroup", 5);
            model.Result.MostCommented = (mostCommentedBlogsFromRepo.Select(item => _mapper.Map<BlogForReturnDto>(item))).ToList();
            //BlogGroups
            var blogGroupFromRepo = await _db.BlogGroupRepository.GetAllAsync();
            model.Result.BlogGroups = _mapper.Map<List<BlogGroupForReturnDto>>(blogGroupFromRepo);

            model.Status = true;
            model.Message = "اطلاعات با موفقیت بارگزاری شد";
            return Ok(model);

        }
        [HttpGet(ApiV1Routes.SiteBlog.GetBlog)]
        [ProducesResponseType(typeof(ApiReturn<BlogsReturnDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetBlog(long blogId)
        {
            var model = new ApiReturn<BlogsReturnDto>
            {
                Result = new BlogsReturnDto()
            };
            var blogFromRepo = await _db.BlogRepository.GetByIdAsync(blogId);

            if (blogFromRepo != null)
            {

                blogFromRepo.ViewCount += 1;

                _db.BlogRepository.Update(blogFromRepo);
                await _db.SaveAsync();


                var blog = _mapper.Map<BlogForReturnDto>(blogFromRepo);

                return Ok(blog);
            }
            else
            {
                return BadRequest("بلاگ وجود ندارد");
            }

        }
    }
}
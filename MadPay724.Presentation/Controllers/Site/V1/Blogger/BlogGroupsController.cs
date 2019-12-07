using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.BlogGroup;
using MadPay724.Data.Models.MainDB.Blog;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Blogger
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Blog")]
    [ApiController]
    public class BlogGroupsController : ControllerBase
    {

        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogGroupsController> _logger;

        public BlogGroupsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<BlogGroupsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        [Authorize(Policy = "AccessBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.BlogGroup.GetBlogGroups)]
        public async Task<IActionResult> GetBlogGroups(string userId)
        {
            var blogGroupsFromRepo = (await _db.BlogGroupRepository
            .GetAllAsync()).OrderByDescending(x => x.DateModified);

            var blogGroups = _mapper.Map<List<BlogGroupForReturnDto>>(blogGroupsFromRepo);

            return Ok(blogGroups);
        }

        [Authorize(Policy = "AccessBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.BlogGroup.GetBlogGroup, Name = "GetBlogGroup")]
        public async Task<IActionResult> GetBlogGroup(string id, string userId)
        {
            var blogGroupFromRepo = await _db.BlogGroupRepository.GetByIdAsync(id);
            if (blogGroupFromRepo != null)
            {
                var blogGroup = _mapper.Map<BlogGroupForReturnDto>(blogGroupFromRepo);

                return Ok(blogGroup);
            }
            else
            {
                return BadRequest("دسته بلاگ وجود ندارد");
            }
        }
        [Authorize(Policy = "AccessAdminBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpPost(ApiV1Routes.BlogGroup.AddBlogGroup)]
        public async Task<IActionResult> AddBlogGroup(string userId, BlogGroupForCreateUpdateDto blogGroupForCreateDto)
        {
            var blogGroupFromRepo = await _db.BlogGroupRepository
                .GetAsync(p => p.Name == blogGroupForCreateDto.Name);

            if (blogGroupFromRepo == null)
            {
                var cardForCreate = new BlogGroup();
                var blogGroup = _mapper.Map(blogGroupForCreateDto, cardForCreate);

                await _db.BlogGroupRepository.InsertAsync(blogGroup);

                if (await _db.SaveAsync())
                {
                    var blogGroupForReturn = _mapper.Map<BlogGroupForReturnDto>(blogGroup);

                    return CreatedAtRoute("GetBlogGroup", new { id = blogGroup.Id, userId = userId }, blogGroupForReturn);
                }
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("این دسته بلاگ قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "AccessAdminBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpPut(ApiV1Routes.BlogGroup.UpdateBlogGroup)]
        public async Task<IActionResult> UpdateBlogGroup(string id, string userId, BlogGroupForCreateUpdateDto blogGroupForUpdateDto)
        {

            var epFromRepo = await _db.BlogGroupRepository
              .GetAsync(p => p.Name == blogGroupForUpdateDto.Name && p.Id != id);
            if (epFromRepo == null)
            {
                var blogGroupFromRepo = await _db.BlogGroupRepository.GetByIdAsync(id);
                if (blogGroupFromRepo != null)
                {
                    var blogGroup = _mapper.Map(blogGroupForUpdateDto, blogGroupFromRepo);
                    blogGroup.DateModified = DateTime.Now;
                    _db.BlogGroupRepository.Update(blogGroup);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                {
                    return BadRequest("دسته بلاگ وجود ندارد");
                }
            }
            {
                return BadRequest("این دسته بلاگ قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "AccessAdminBlog")]
        [HttpDelete(ApiV1Routes.BlogGroup.DeleteBlogGroup)]
        public async Task<IActionResult> DeleteBlogGroup(string id, string userId)
        {
            var blogGroupFromRepo = (await _db.BlogGroupRepository.GetManyAsync(
                p => p.Id == id, null, "Blogs")).SingleOrDefault();
            if (blogGroupFromRepo != null)
            {
                var parentFromRepo = await _db.BlogGroupRepository.GetManyAsync(
                p => p.Parent == id, null, "");

                if (parentFromRepo.Any())
                {
                    return BadRequest("امکان حذف دسته بندی هایی که  زیر مجموعه دارند را ندارید، ابتدا دسته بندی های زیر مجموعه را حذف کنید");
                }
                else
                {
                    if (blogGroupFromRepo.Blogs.Count > 0)
                    {
                        return BadRequest("امکان حذف دسته بدنی هایی که دارای بلاگ هستند نیست");
                    }
                    else
                    {
                        _db.BlogGroupRepository.Delete(blogGroupFromRepo);

                        if (await _db.SaveAsync())
                            return NoContent();
                        else
                            return BadRequest("خطا در حذف اطلاعات");
                    }
                }               
            }
            else
            {
                return BadRequest("دسته بلاگ وجود ندارد");
            }
        }
    }
}
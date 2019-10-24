using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Models.Blog;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.BlogAdmin
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Blog")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<BlogsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "AccessBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Blog.GetBlogs)]
        public async Task<IActionResult> GetBlogs(string userId)
        {
            if (User.HasClaim(ClaimTypes.Role, "AdminBlog") || User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var blogsFromRepo = await _db.BlogRepository
                    .GetManyAsync(null, s => s.OrderByDescending(x => x.DateModified), "Users");

                var bankcards = _mapper.Map<List<BlogForReturnDto>>(blogsFromRepo);

                return Ok(bankcards);
            }
            else
            {
                var blogsFromRepo = await _db.BlogRepository
                    .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.DateModified), "Users");

                var bankcards = _mapper.Map<List<BlogForReturnDto>>(blogsFromRepo);

                return Ok(bankcards);
            }

        }

        [Authorize(Policy = "AccessBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Blog.GetBlog, Name = "GetBlog")]
        public async Task<IActionResult> GetBlog(string id, string userId)
        {
            if (User.HasClaim(ClaimTypes.Role, "AdminBlog") || User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var blogFromRepo =
                 (await _db.BlogRepository
                 .GetManyAsync(p => p.Id == id, s => s.OrderByDescending(x => x.DateModified), "Users")).FirstOrDefault();

                if (blogFromRepo != null)
                {
                    var blog = _mapper.Map<BlogForReturnDto>(blogFromRepo);

                    return Ok(blog);
                }
                else
                {
                    return BadRequest("بلاگ وجود ندارد");
                }
            }
            else
            {
                var blogFromRepo =
                    (await _db.BlogRepository
                    .GetManyAsync(p => p.Id == id, s => s.OrderByDescending(x => x.DateModified), "Users")).FirstOrDefault();

                if (blogFromRepo != null)
                {
                    if (blogFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    {
                        var blog = _mapper.Map<BlogForReturnDto>(blogFromRepo);

                        return Ok(blog);
                    }
                    else
                    {
                        _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به بلاگ دیگری را دارد");

                        return BadRequest("شما اجازه دسترسی به بلاگ کاربر دیگری را ندارید");
                    }
                }
                else
                {
                    return BadRequest("بلاگ وجود ندارد");
                }
            }


        }
        [Authorize(Policy = "AccessBlog")]
        [HttpPost(ApiV1Routes.Blog.AddBlog)]
        public async Task<IActionResult> AddBlog(string userId, BlogForCreateUpdateDto blogForCreateDto)
        {
            blogForCreateDto.Title = blogForCreateDto.Title.Trim();

            var blogFromRepo = await _db.BlogRepository
             .GetAsync(p => p.Title == blogForCreateDto.Title);

            if (blogFromRepo == null)
            {
                var cardForCreate = new Blog()
                {
                    UserId = userId,
                };
                var blog = _mapper.Map(blogForCreateDto, cardForCreate);

                await _db.BlogRepository.InsertAsync(blog);

                if (await _db.SaveAsync())
                {
                    var blogForReturn = _mapper.Map<BlogForReturnDto>(blog);

                    return CreatedAtRoute("GetBlog", new { id = blog.Id, userId = userId }, blogForReturn);
                }
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest(" بلاگی با این تایتل قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "AccessBlog")]
        [HttpPut(ApiV1Routes.Blog.UpdateBlog)]
        public async Task<IActionResult> UpdateBlog(string id, string userId, BlogForCreateUpdateDto blogForUpdateDto)
        {
            if (User.HasClaim(ClaimTypes.Role, "AdminBlog") || User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var epFromRepo = await _db.BlogRepository
               .GetAsync(p => p.Title == blogForUpdateDto.Title && p.Id != id);
                if (epFromRepo == null)
                {
                    var blogFromRepo = await _db.BlogRepository.GetByIdAsync(id);
                    if (blogFromRepo != null)
                    {
                        var blog = _mapper.Map(blogForUpdateDto, blogFromRepo);
                        blog.DateModified = DateTime.Now;
                        _db.BlogRepository.Update(blog);

                        if (await _db.SaveAsync())
                            return NoContent();
                        else
                            return BadRequest("خطا در ثبت اطلاعات");
                    }
                    {
                        return BadRequest("بلاگ وجود ندارد");
                    }
                }
                {
                    return BadRequest("این بلاگ قبلا ثبت شده است");
                }
            }
            else
            {
                var epFromRepo = await _db.BlogRepository
                .GetAsync(p => p.Title == blogForUpdateDto.Title && p.Id != id);
                if (epFromRepo == null)
                {
                    var blogFromRepo = await _db.BlogRepository.GetByIdAsync(id);
                    if (blogFromRepo != null)
                    {
                        if (blogFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                        {
                            var blog = _mapper.Map(blogForUpdateDto, blogFromRepo);
                            blog.DateModified = DateTime.Now;
                            _db.BlogRepository.Update(blog);

                            if (await _db.SaveAsync())
                                return NoContent();
                            else
                                return BadRequest("خطا در ثبت اطلاعات");
                        }
                        else
                        {
                            _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به بلاگ دیگری را دارد");

                            return BadRequest("شما اجازه اپدیت بلاگ کاربر دیگری را ندارید");
                        }
                    }
                    {
                        return BadRequest("بلاگ وجود ندارد");
                    }
                }
                {
                    return BadRequest("این بلاگ قبلا ثبت شده است");
                }
            }



        }
        [Authorize(Policy = "AccessAdminBlog")]
        [HttpPut(ApiV1Routes.Blog.ApproveBlog)]
        public async Task<IActionResult> ApproveBlog(string id, string userId, ApproveSelectBlogDto approveSelectBlogDto)
        {
            var blogFromRepo = await _db.BlogRepository.GetByIdAsync(id);
            if (blogFromRepo != null)
            {
                blogFromRepo.DateModified = DateTime.Now;
                blogFromRepo.Status = approveSelectBlogDto.Flag;
                _db.BlogRepository.Update(blogFromRepo);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("بلاگ وجود ندارد");
            }
        }
        [Authorize(Policy = "AccessAdminBlog")]
        [HttpPut(ApiV1Routes.Blog.SelectBlog)]
        public async Task<IActionResult> SelectBlog(string id, string userId, ApproveSelectBlogDto approveSelectBlogDto)
        {
            var blogFromRepo = await _db.BlogRepository.GetByIdAsync(id);
            if (blogFromRepo != null)
            {
                blogFromRepo.DateModified = DateTime.Now;
                blogFromRepo.IsSelected = approveSelectBlogDto.Flag;
                _db.BlogRepository.Update(blogFromRepo);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("بلاگ وجود ندارد");
            }
        }

        [Authorize(Policy = "AccessAdminBlog")]
        [HttpDelete(ApiV1Routes.Blog.DeleteBlog)]
        public async Task<IActionResult> DeleteBlog(string id, string userId)
        {
            var blogFromRepo = await _db.BlogRepository.GetByIdAsync(id);
            if (blogFromRepo != null)
            {
                _db.BlogRepository.Delete(blogFromRepo);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در حذف اطلاعات");
            }
            else
            {
                return BadRequest("بلاگ وجود ندارد");
            }
        }
    }
}
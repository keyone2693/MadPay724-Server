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

namespace MadPay724.Presentation.Controllers.Site.V1.Blogger
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Blog")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogsController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IUtilities _utilities;

        public BlogsController(IUnitOfWork<Main_MadpayDbContext> dbContext, IMapper mapper,
            ILogger<BlogsController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IUtilities utilities)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _utilities = utilities;
        }
        [Authorize(Policy = "AccessBlog")]
        [HttpPost(ApiV1Routes.Blog.UploadBlogImage)]
        public async Task<IActionResult> UploadBlogImage(IFormFile UploadFiles)
        {
            var creatDir = _uploadService.CreateDirectory(_env.WebRootPath,
                 "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day);

            if (creatDir.status)
            {
                var uploadRes = await _uploadService.UploadFileToLocal(
                      UploadFiles,
                      Guid.NewGuid().ToString(),
                      _env.WebRootPath,
                      $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                      "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day
                  );
                if (uploadRes.Status)
                {
                    Response.Headers.Add("ejUrl", uploadRes.Url);
                    Response.Headers.Add("Access-Control-Expose-Headers", "ejUrl");
                    return Ok();
                }
                else
                {
                    return BadRequest(new CkEditorUploadImgReaturnMesssage
                    {
                        uploaded = false
                    });
                }
            }
            else
            {
                return BadRequest(new CkEditorUploadImgReaturnMesssage
                {
                    uploaded = false
                });
            }

        }
        [Authorize(Policy = "AccessBlog")]
        [HttpPost(ApiV1Routes.Blog.DeleteBlogImage)]
        public async Task<IActionResult> DeleteBlogImage(BlogImgDeletingDto blogImgDeletingDto)
        {
            var deleteRes = _uploadService.RemoveFileFromLocal(
                  Path.GetFileName(blogImgDeletingDto.ImgUrl),
                  _env.WebRootPath,
                  _utilities.FindLocalPathFromUrl(blogImgDeletingDto.ImgUrl).Replace("wwwroot\\", ""));
            if (deleteRes.Status)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new CkEditorUploadImgReaturnMesssage
                {
                    uploaded = false
                });
            }

        }
        [Authorize(Policy = "AccessBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Blog.GetBlogs)]
        public async Task<IActionResult> GetBlogs(string userId, [FromQuery]PaginationDto paginationDto)
        {
            if (User.HasClaim(ClaimTypes.Role, "AdminBlog") || User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var blogsFromRepo = await _db.BlogRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToBlogExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "User,BlogGroup");

                Response.AddPagination(blogsFromRepo.CurrentPage, blogsFromRepo.PageSize,
                    blogsFromRepo.TotalCount, blogsFromRepo.TotalPage);

                var blogs = new List<BlogForReturnDto>();

                foreach (var item in blogsFromRepo)
                {
                    blogs.Add(_mapper.Map<BlogForReturnDto>(item));
                }

                return Ok(blogs);
            }
            else
            {
                var blogsFromRepo = await _db.BlogRepository
           .GetAllPagedListAsync(
                    paginationDto,
           paginationDto.Filter.ToBlogExpression(false, userId),
           paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
           "User,BlogGroup");

                Response.AddPagination(blogsFromRepo.CurrentPage, blogsFromRepo.PageSize,
                    blogsFromRepo.TotalCount, blogsFromRepo.TotalPage);

                var blogs = new List<BlogForReturnDto>();

                foreach (var item in blogsFromRepo)
                {
                    blogs.Add(_mapper.Map<BlogForReturnDto>(item));
                }


                return Ok(blogs);
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
                 .GetManyAsync(p => p.Id == id, s => s.OrderByDescending(x => x.DateModified), "User")).FirstOrDefault();

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
                    .GetManyAsync(p => p.Id == id, s => s.OrderByDescending(x => x.DateModified), "User")).FirstOrDefault();

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
        public async Task<IActionResult> AddBlog(string userId, [FromForm]BlogForCreateUpdateDto blogForCreateDto)
        {
            blogForCreateDto.Title = blogForCreateDto.Title.Trim();

            var blogFromRepo = await _db.BlogRepository
             .GetAsync(p => p.Title == blogForCreateDto.Title);

            if (blogFromRepo == null)
            {
                var blogForCreate = new Blog()
                {
                    UserId = userId,
                    Status = false,
                    IsSelected = false
                };

                var creatDir = _uploadService.CreateDirectory(_env.WebRootPath,
                    "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day);

                if (creatDir.status)
                {
                    var uploadRes = await _uploadService.UploadFileToLocal(
                      blogForCreateDto.File,
                          blogForCreate.Id,
                          _env.WebRootPath,
                          $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                          "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day
                      );
                    if (uploadRes.Status)
                    {
                        blogForCreate.PicAddress = uploadRes.Url;

                        var blog = _mapper.Map(blogForCreateDto, blogForCreate);

                        await _db.BlogRepository.InsertAsync(blog);

                        if (await _db.SaveAsync())
                        {
                            var blogForReturn = _mapper.Map<BlogForReturnDto>(blog);

                            return CreatedAtRoute("GetBlog", new { id = blog.Id, userId = userId }, blogForReturn);
                        }
                        else
                            return BadRequest("خطا در ثبت اطلاعات");
                    }
                    else
                    {
                        return BadRequest(uploadRes.Message);
                    }
                }
                else
                {
                    return BadRequest(creatDir.message);
                }

            }
            {
                return BadRequest(" بلاگی با این تایتل قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "AccessBlog")]
        [HttpPut(ApiV1Routes.Blog.UpdateBlog)]
        public async Task<IActionResult> UpdateBlog(string id, string userId, [FromForm]BlogForCreateUpdateDto blogForUpdateDto)
        {
            if (User.HasClaim(ClaimTypes.Role, "AdminBlog") || User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                blogForUpdateDto.Title = blogForUpdateDto.Title.Trim();

                var epFromRepo = await _db.BlogRepository
               .GetAsync(p => p.Title == blogForUpdateDto.Title && p.Id != id);
                if (epFromRepo == null)
                {
                    var blogForUpdate = _db.BlogRepository.GetById(id);
                    blogForUpdate.DateModified = DateTime.Now;

                    if (_utilities.IsFile(blogForUpdateDto.File))
                    {
                        var deleteRes = _uploadService.RemoveFileFromLocal(
                          Path.GetFileName(blogForUpdate.PicAddress),
                          _env.WebRootPath,
                          _utilities.FindLocalPathFromUrl(blogForUpdate.PicAddress).Replace("wwwroot\\", ""));
                        if (deleteRes.Status)
                        {
                            var creatDir = _uploadService.CreateDirectory(_env.WebRootPath,
                                "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day);

                            if (creatDir.status)
                            {
                                var uploadRes = await _uploadService.UploadFileToLocal(
                                  blogForUpdateDto.File,
                                      id,
                                      _env.WebRootPath,
                                      $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                                      "Files\\Blog\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day
                                  );
                                if (uploadRes.Status)
                                {
                                    blogForUpdate.PicAddress = uploadRes.Url;

                                    var blog = _mapper.Map(blogForUpdateDto, blogForUpdate);

                                    _db.BlogRepository.Update(blog);

                                    if (await _db.SaveAsync())
                                    {
                                        return NoContent();
                                    }
                                    else
                                        return BadRequest("خطا در ویرایش اطلاعات");
                                }
                                else
                                {
                                    return BadRequest(uploadRes.Message);
                                }
                            }
                            else
                            {
                                return BadRequest(creatDir.message);
                            }
                        }
                        else
                        {
                            return BadRequest("خطا در حذف عکس قبلی");
                        }
                    }
                    else
                    {
                        var blog = _mapper.Map(blogForUpdateDto, blogForUpdate);

                        _db.BlogRepository.Update(blog);

                        if (await _db.SaveAsync())
                        {
                            return NoContent();
                        }
                        else
                            return BadRequest("خطا در ویرایش اطلاعات");
                    }

                }
                {
                    return BadRequest("این بلاگ قبلا ثبت شده است");
                }
            }
            else
            {

                return Unauthorized("اجازه ویرایش ندارید");
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

                _uploadService.RemoveFileFromLocal(blogFromRepo.PicAddress.Split('/').Last(),
                    _env.WebRootPath,
                    _utilities.FindLocalPathFromUrl(blogFromRepo.PicAddress).Replace("wwwroot\\", ""));

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

        //------------------------------
        [Authorize(Policy = "AccessAdminBlog")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Blog.GetUnverifiedBlogCount)]
        public async Task<IActionResult> GetUnverifiedBlogCount(string id)
        {

            var count =
             await _db.BlogRepository.GetCountAsync(p => !p.Status);

            return Ok(count);
        }
    }
}
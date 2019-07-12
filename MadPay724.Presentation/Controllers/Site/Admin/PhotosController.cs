
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "Site")]
    [Route("site/admin/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;

        public PhotosController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper, IUploadService uploadService,
             IWebHostEnvironment env)
        {
            _env = env;
            _db = dbContext;
            _mapper = mapper;
            _uploadService = uploadService;
        }
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(string id)
        {
            var photoFromRepo = await _db.PhotoRepository.GetByIdAsync(id);

            var photo = _mapper.Map<PhotoForReturnProfileDto>(photoFromRepo);

            return Ok(photo);
        }

        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpPost]
        public async Task<IActionResult> ChangeUserPhoto(string userId, [FromForm]PhotoForProfileDto photoForProfileDto)
        {
            //var userFromRepo = await _db.UserRepository.GetByIdAsync(userId);

            // var uplaodRes = _uploadService.UploadToCloudinary(photoForProfileDto.File);

            var uplaodRes = await _uploadService.UploadProfilePic(
                photoForProfileDto.File,
                userId,
                _env.WebRootPath ,
                string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value)
                );

            if (uplaodRes.Status)
            {
                photoForProfileDto.Url = uplaodRes.Url;
                if(uplaodRes.LocalUploaded)
                    photoForProfileDto.PublicId = "1";
                else
                    photoForProfileDto.PublicId = uplaodRes.PublicId;



                var oldphoto = await _db.PhotoRepository.GetAsync(p => p.UserId == userId && p.IsMain);

                if (oldphoto.PublicId != null && oldphoto.PublicId != "0" && oldphoto.PublicId != "1")
                {
                    _uploadService.RemoveFileFromCloudinary(oldphoto.PublicId);
                }
                if (oldphoto.PublicId == photoForProfileDto.PublicId && photoForProfileDto.Url.Split('/').Last() != oldphoto.Url.Split('/').Last())
                {
                    _uploadService.RemoveFileFromLocal(oldphoto.Url.Split('/').Last(), _env.WebRootPath, "Files\\Pic\\Profile");
                }

                if (oldphoto.PublicId == "1" && photoForProfileDto.PublicId != "1")
                {
                    _uploadService.RemoveFileFromLocal(oldphoto.Url.Split('/').Last(), _env.WebRootPath, "Files\\Pic\\Profile");
                }

                _mapper.Map(photoForProfileDto, oldphoto);

                _db.PhotoRepository.Update(oldphoto);

                if (await _db.SaveAsync())
                {
                    var photoForReturn = _mapper.Map<PhotoForReturnProfileDto>(oldphoto);
                    return CreatedAtRoute("GetPhoto", routeValues: new { id = oldphoto.Id }, value: photoForReturn);
                }
                else
                {
                    return BadRequest("خطایی در اپلود دوباره امتحان کنید");
                }
            }
            else
            {
                return BadRequest(uplaodRes.Message);
            }
        }

    


    }
}
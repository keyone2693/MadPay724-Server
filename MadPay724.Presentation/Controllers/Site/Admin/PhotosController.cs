using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _db = dbContext;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
               _cloudinaryConfig.Value.CloudName,
               _cloudinaryConfig.Value.APIKey,
                _cloudinaryConfig.Value.APISecret
           );

            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(string id)
        {
            var photoFromRepo = await _db.PhotoRepository.GetByIdAsync(id);

            var photo = _mapper.Map<PhotoForReturnProfileDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserPhoto(string userId, [FromForm]PhotoForProfileDto photoForProfileDto)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Unauthorized("شما اجازه تغییر تصویر این کاربر را ندارید");
            }

            //var userFromRepo = await _db.UserRepository.GetByIdAsync(userId);


            var file = photoForProfileDto.File;

            var updaodResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uplaodParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(150).Height(150).Crop("fill").Gravity("face")
                    };

                    updaodResult = _cloudinary.Upload(uplaodParams);
                }
            }
            
            photoForProfileDto.Url = updaodResult.Uri.ToString();
            photoForProfileDto.PublicId = updaodResult.PublicId;


            var oldphoto = await _db.PhotoRepository.GetAsync(p=>p.UserId == userId && p.IsMain);

            if(oldphoto.PublicId != null && oldphoto.PublicId != "0")
            {
                var deleteParams = new DeletionParams(oldphoto.PublicId);
                var deleteResult = _cloudinary.Destroy(deleteParams);
                //if(deleteResult.Result.ToLower() == "ok")
                //{

                //}
            }

            _mapper.Map(photoForProfileDto, oldphoto);

            _db.PhotoRepository.Update(oldphoto);

            if(await _db.SaveAsync())
            {
                var photoForReturn = _mapper.Map<PhotoForReturnProfileDto>(oldphoto);
                return CreatedAtRoute("GetPhoto",routeValues:  new { id = oldphoto.Id },value:  photoForReturn);
            }
            else
            {
                return BadRequest("خطایی در اپلود دوباره امتحان کنید");
            }
        }

    


    }
}
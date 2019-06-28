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
    [Route("site/admin/{userId}/photos")]
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

        [HttpPost]
        public async Task<IActionResult> ChangeUserPhoto(string userId, PhotoForProfileDto photoForProfileDto)
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
                        Transformation = new Transformation().Width(250).Height(250).Crop("fill").Gravity("face")
                    };

                    updaodResult = _cloudinary.Upload(uplaodParams);
                }
            }


            photoForProfileDto.Url = updaodResult.Uri.ToString();
            photoForProfileDto.PublicId = updaodResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForProfileDto);
            photo.IsMain = true;
            photo.UserId = userId;

            await _db.PhotoRepository.InsertAsync(photo);

            if(await _db.SaveAsync())
            {
                return Ok();
            }
            else
            {
                return BadRequest("خطایی در اپلود دوباره اتحان کنید");
            }
        }


    }
}
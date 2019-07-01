using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Upload.Service
{
    public class UploadService : IUploadService
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly Cloudinary _cloudinary;
        private readonly Setting _setting;
        public UploadService(IUnitOfWork<MadpayDbContext> dbContext)
        {
            _db = dbContext;

            _setting = _db.SettingRepository.GetById(1);
            Account acc = new Account(
              _setting.CloudinaryCloudName,
              _setting.CloudinaryAPIKey,
               _setting.CloudinaryAPISecret
          );

            _cloudinary = new Cloudinary(acc);
        }
        public Task<FileUploadedDto> UploadFile(IFormFile File)
        {
            throw new NotImplementedException();
        }



        public Task<FileUploadedDto> UploadToLocal(IFormFile File)
        {
            throw new NotImplementedException();
        }

        public FileUploadedDto UploadToCloudinary(IFormFile file)
        {
            var updaodResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uplaodParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(150).Height(150).Crop("fill").Gravity("face")
                        };
                        updaodResult = _cloudinary.Upload(uplaodParams);
                        if (string.IsNullOrEmpty(updaodResult.Error.Message))
                        {
                            return new FileUploadedDto()
                            {
                                Status = true,
                                Message = "با موفقیت در فضای ابری آپلود شد",
                                PublicId = updaodResult.PublicId,
                                Url = updaodResult.Uri.ToString()
                            };
                        }
                        return new FileUploadedDto()
                        {
                            Status = false,
                            Message = updaodResult.Error.Message
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new FileUploadedDto()
                    {
                        Status = false,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false,
                    Message = "فایلی برای اپلود یافت نشد"
                };
            }
        }
    }
}

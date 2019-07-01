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
using System.IO;
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
        public Task<FileUploadedDto> UploadFile(IFormFile file)
        {
            throw new NotImplementedException();
        }



        public async Task<FileUploadedDto> UploadToLocal(IFormFile file, string WebRootPath , string UrlBegan)
        {

            if (file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtention = Path.GetExtension(fileName);
                    string fileNewName = string.Format("{0}{1}", Guid.NewGuid(), fileExtention);
                    string path = Path.Combine(WebRootPath, "Files/Pic/Profile");
                    string fullPath = Path.Combine(path, fileNewName);

                    using(var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return new FileUploadedDto()
                    {
                        Status = true,
                        Message = "با موفقیت در فضای ابری آپلود شد",
                        PublicId = "0",
                        Url = string.Format("{0}/{1}", UrlBegan, "wwwroot/Files/Pic/profilepic.png")
                    };
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

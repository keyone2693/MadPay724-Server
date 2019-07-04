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

            _setting = _db.SettingRepository.GetById((short)1);
            Account acc = new Account(
              _setting.CloudinaryCloudName,
              _setting.CloudinaryAPIKey,
               _setting.CloudinaryAPISecret
          );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<FileUploadedDto> UploadProfilePic(IFormFile file, string userId, string WebRootPath, string UrlBegan)
        {
            if(_setting.UploadLocal)
            {
               return  await UploadProfilePicToLocal(file, userId, WebRootPath, UrlBegan);
            }
            else
            {
                return UploadProfilePicToCloudinary(file, userId);
            }
        }

        public async Task<FileUploadedDto> UploadProfilePicToLocal(IFormFile file, string userId, string WebRootPath, string UrlBegan)
        {

            if (file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtention = Path.GetExtension(fileName);
                    string fileNewName = string.Format("{0}{1}", userId, fileExtention);
                    string path = Path.Combine(WebRootPath, "Files\\Pic\\Profile");
                    string fullPath = Path.Combine(path, fileNewName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return new FileUploadedDto()
                    {
                        Status = true,
                        LocalUploaded = true,
                        Message = "با موفقیت در لوکال آپلود شد",
                        PublicId = "0",
                        Url = string.Format("{0}/{1}", UrlBegan, "wwwroot/Files/Pic/Profile/" + fileNewName)
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

        public FileUploadedDto UploadProfilePicToCloudinary(IFormFile file, string userId)
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
                            Transformation = new Transformation().Width(150).Height(150).Crop("fill").Gravity("face"),
                            Folder = "ProfilePic/" + userId
                        };
                        updaodResult = _cloudinary.Upload(uplaodParams);
                        if (updaodResult.Error == null)
                        {
                            return new FileUploadedDto()
                            {
                                Status = true,
                                LocalUploaded = false,
                                Message = "با موفقیت در فضای ابری آپلود شد",
                                PublicId = updaodResult.PublicId,
                                Url = updaodResult.Uri.ToString()
                            };
                        }
                        else
                        {
                            return new FileUploadedDto()
                            {
                                Status = false,
                                Message = updaodResult.Error.Message
                            };
                        }

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

        public FileUploadedDto RemoveFileFromCloudinary(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = _cloudinary.Destroy(deleteParams);
            if (deleteResult.Result.ToLower() == "ok")
            {
                return new FileUploadedDto()
                {
                    Status = true,
                    Message = "فایل با موفقیت حذف شد"
                };
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false
                };
            }
        }

        public FileUploadedDto RemoveFileFromLocal(string photoName, string WebRootPath, string filePath)
        {

            string path = Path.Combine(WebRootPath, filePath);
            string fullPath = Path.Combine(path, photoName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return new FileUploadedDto()
                {
                    Status = true,
                    Message = "فایل با موفقیت حذف شد"
                };
            }
            else{
                return new FileUploadedDto()
                {
                    Status = true,
                    Message = "فایل وجود نداشت"
                };
            }
        }
    }
}

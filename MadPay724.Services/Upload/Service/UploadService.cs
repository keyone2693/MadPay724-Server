using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Upload.Service
{
    public class UploadService : IUploadService
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly Cloudinary _cloudinary;
        private readonly Setting _setting;
        public UploadService(IUnitOfWork<Main_MadPayDbContext> dbContext)
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
        public async Task<FileUploadedDto> UploadFile(IFormFile file, string userId, string WebRootPath, string UrlBegan)
        {
            if(_setting.UploadLocal)
            {
               return  await UploadFileToLocal(file, userId, WebRootPath, UrlBegan);
            }
            else
            {
                return UploadFileToCloudinary(file, userId);
            }
        }

        public async Task<FileUploadedDto> UploadFileToLocal(IFormFile file, string userId,
            string WebRootPath, string UrlBegan, string Url = "Files\\Pic\\Profile")
        {

            if (file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtention = Path.GetExtension(fileName);
                    string fileNewName = string.Format("{0}{1}", userId, fileExtention);
                    string path = Path.Combine(WebRootPath, Url);
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
                        Url = $"{UrlBegan}/{"wwwroot/" + Url.Split('\\').Aggregate("", (current, str) => current + (str + "/")) + fileNewName}"
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

        public FileUploadedDto UploadFileToCloudinary(IFormFile file, string userId)
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

        public ReturnMessage CreateDirectory(string WebRootPath, string Url)
        {
            try
            {
                var path = Path.Combine(WebRootPath, Url);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return new ReturnMessage
                {
                    status = true
                };

            }
            catch (Exception ex)
            {
                return new ReturnMessage
                {
                    status = false,
                    message = ex.Message
                };
            }
            
        }

    }
}

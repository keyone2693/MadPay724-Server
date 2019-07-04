using MadPay724.Data.Dtos.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Upload.Interface
{
    public interface IUploadService
    {
        FileUploadedDto UploadProfilePicToCloudinary(IFormFile file, string userId);
        Task<FileUploadedDto> UploadProfilePicToLocal(IFormFile file, string userId, string WebRootPath, string UrlBegan);
        Task<FileUploadedDto> UploadProfilePic(IFormFile file, string userId, string WebRootPath, string UrlBegan);
        FileUploadedDto RemoveFileFromCloudinary(string publicId);
    }
}

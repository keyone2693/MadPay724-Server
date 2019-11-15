using MadPay724.Common.ErrorAndMessage;
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
        FileUploadedDto UploadFileToCloudinary(IFormFile file, string userId);
        Task<FileUploadedDto> UploadFileToLocal(IFormFile file, string userId, string WebRootPath, string UrlBegan, string UrlUrl = "Files\\Pic\\Profile");
        Task<FileUploadedDto> UploadFile(IFormFile file, string userId, string WebRootPath, string UrlBegan);
        FileUploadedDto RemoveFileFromCloudinary(string publicId);
        FileUploadedDto RemoveFileFromLocal(string photoName, string WebRootPath , string filePath);

        ReturnMessage CreateDirectory(string WebRootPath, string UrlUrl);
    }
}

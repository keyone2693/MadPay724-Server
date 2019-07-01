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
        FileUploadedDto UploadToCloudinary(IFormFile file);
        Task<FileUploadedDto> UploadToLocal(IFormFile file);
        Task<FileUploadedDto> UploadFile(IFormFile file);
    }
}

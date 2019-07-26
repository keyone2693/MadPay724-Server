using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MadPay724.Test.DataInput
{
    public static class UnitTestsDataInput
    {

        public const string baseRouteV1 = "api/v1/";

        public const string unToken = "";

        public const string aToken =
            "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzODhkZTJiYy04NTFkLTRjOTUtOGJmOS0xOTM5ZTUyZTQ0YzgiLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2NDE0NjA2OSwiZXhwIjoxNTY0MjMyNDY5LCJpYXQiOjE1NjQxNDYwNjl9.oy4WZfWUOJYNshy6YUk681ZHMlnbf30l0x_Jm98LHIbwDccei_R2WpPz92k2pR7r9m6MUaiI9AXDVYk5REWBaQ";



        public const string userLogedInUsername = "kathybrown@barkarama.com";
        public const string userLogedInPassword = "password";
        public const string userLogedInId = "1e15701a-8291-420d-923a-eebf5f2b6fc5";
        public const string userAnOtherId = "388de2bc-851d-4c95-8bf9-1939e52e44c8";
        public const string userLogedInPhotoId = "e97fd389-fb3d-4ea2-929d-435f5ecdc159";


        public static readonly IEnumerable<User> Users = new List<User>()
        {
            new User
            {
                Id = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
                DateOfBirth = DateTime.Now,
                LastActive = DateTime.Now,
                PasswordHash ="",
                UserName = "kathybrown@barkarama.com",
                Name = "Holloway Vasquez",
                PhoneNumber = "55",
                Address = "55",
                Gender = true,
                City = "55",
                IsAcive = true,
                Status = true,
                Photos = new List<Photo>()
                {
                    new Photo()
                    {
                        Id = "0d47394e-672f-4db7-898c-bfd8f32e2af",
                        UserId = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        PublicId = "1",
                        Url = "qq",
                        Alt = "qq",
                        IsMain = true,
                        Description = "qq",
                    }
                }
            }
        };

        public static readonly Setting settingForUpload = new Setting()
        {
            CloudinaryCloudName="12",
            CloudinaryAPIKey = "12",
            CloudinaryAPISecret = "12"
        };

        public static readonly UserForDetailedDto userForDetailedDto = new UserForDetailedDto()
        {
            Id = "0d47394e-672f-4db7-898c-bfd8f32e2af7",
            UserName = "haysmathis@barkarama.com",
            Name = "Holloway Vasquez",
            PhoneNumber = "55",
            Address = "55",
            Gender = true,
            City = "55",
            Age = 15,
            LastActive = DateTime.Now,
            PhotoUrl = "qqq"
        };


        public static readonly UserForRegisterDto userForRegisterDto = new UserForRegisterDto()
        {
            UserName = "asasas@b545ma.com",
            Password = "123789",
            Name = "کیوان",
            PhoneNumber = "15486523"
        };

        public static readonly UserForRegisterDto userForRegisterDto_Fail_Exist = new UserForRegisterDto()
        {
            UserName = "kathybrown@barkarama.com",
            Password = "123789",
            Name = "کیوان",
            PhoneNumber = "15486523"
        };

        public static readonly UserForRegisterDto userForRegisterDto_Fail_ModelState = new UserForRegisterDto()
        {
            UserName = string.Empty,
            Password = string.Empty,
            Name = string.Empty,
            PhoneNumber = string.Empty
        };

        public static readonly UseForLoginDto useForLoginDto_Success = new UseForLoginDto()
        {
            UserName = "kathybrown@barkarama.com",
            Password = "password",
            IsRemember = true
        };

        public static readonly UseForLoginDto useForLoginDto_Fail = new UseForLoginDto()
        {
            UserName = "00@000.com",
            Password = "0000",
            IsRemember = true
        };

        public static readonly UseForLoginDto useForLoginDto_Fail_ModelState = new UseForLoginDto()
        {
            UserName = string.Empty,
            Password = string.Empty
        };

        public static readonly PhotoForProfileDto photoForProfileDto = new PhotoForProfileDto()
        {
            Url = "http://google.com",
            PublicId = "1"
        };

        public static readonly UserForUpdateDto userForUpdateDto = new UserForUpdateDto()
        {
            Name = "علی حسینی",
            PhoneNumber = "string",
            Address = "string",
            Gender = true,
            City = "string"
        };

        public static readonly UserForUpdateDto userForUpdateDto_Fail_ModelState = new UserForUpdateDto()
        {
            Name = string.Empty,
            PhoneNumber = string.Empty,
            Address = string.Empty,
            City =
                "لورم ایپسوم متن ساختگی با تولید سادگلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی دری نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی در."

        };

        public static readonly PasswordForChangeDto passwordForChangeDto = new PasswordForChangeDto()
        {
            OldPassword = "123789",
            NewPassword = "123789"
        };

        public static readonly PasswordForChangeDto passwordForChangeDto_Fail = new PasswordForChangeDto()
        {
            OldPassword = "123789654645",
            NewPassword = "123789"
        };

        public static readonly PasswordForChangeDto passwordForChangeDto_Fail_ModelState = new PasswordForChangeDto()
        {
            OldPassword = string.Empty,
            NewPassword = string.Empty
        };

        public static readonly UserForUpdateDto userForUpdateDto_Fail = new UserForUpdateDto()
        {
            Name = "kldlsdnf"
        };

        public static readonly PasswordForChangeDto passwordForChangeDto_Success = new PasswordForChangeDto()
        {
            OldPassword = It.IsAny<string>(),
            NewPassword = It.IsAny<string>()
        };



        public static readonly PhotoForReturnProfileDto PhotoForReturnProfileDto = new PhotoForReturnProfileDto()
        {};

        public static readonly FileUploadedDto fileUploadedDto_Success = new FileUploadedDto()
        {
            Status = true,
            LocalUploaded = true,
            Message = "با موفقیت در لوکال آپلود شد",
            PublicId = "0",
            Url =  "wwwroot/Files/Pic/Profile/"
        };

        public static readonly FileUploadedDto fileUploadedDto_Fail_WrongFile = new FileUploadedDto()
        {
            Status = false,
            Message = "فایلی برای اپلود یافت نشد"
        };

    }
}

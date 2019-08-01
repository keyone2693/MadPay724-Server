using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Common.Token;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Dtos.Site.Panel.Photos;
using MadPay724.Data.Dtos.Site.Panel.Roles;
using MadPay724.Data.Dtos.Site.Panel.Users;
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
            "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI0MjFhMWFhYi1jNzQ0LTRhMjYtYWIxNC05OTQwMDVlY2U0NmYiLCJ1bmlxdWVfbmFtZSI6ImFkbWluQG1hZHBheTcyNC5jb20iLCJyb2xlIjpbIkJsb2ciLCJBY2NvdW50YW50IiwiQWRtaW4iXSwibmJmIjoxNTY0MzIyMDQxLCJleHAiOjE1NjQ0MDg0NDEsImlhdCI6MTU2NDMyMjA0MX0.G6PINoJ4QO93ihrLw9JQjE1jIQmfiC0VEbnzpCFrEul4xyjDeoILftGHb6dP-aErFLOchEonx0PX5qNqQqT4YQ";



        public const string userLogedInUsername = "keyvan@madpay.com";
        public const string userLogedInPassword = "password";
        public const string userLogedInId = "0b83c5e3-404e-44ea-8013-122b691453fa";
        public const string userAnOtherId = "388de2bc-851d-4c95-8bf9-1939e52e44c8";
        public const string userLogedInPhotoId = "e97fd389-fb3d-4ea2-929d-435f5ecdc159";
        public const string userAnOtherPhotoId = "e97fd389-fb3d-4ea2-929d-435f5e";


        public static readonly IEnumerable<User> Users = new List<User>()
        {
            new User
            {
                Id = "0b83c5e3-404e-44ea-8013-122b691453fa",
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
                        UserId = "0b83c5e3-404e-44ea-8013-122b691453fa",
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

        public static readonly IEnumerable<Role> Roles = new List<Role>()
        {
            new Role()
            {
                Id = "0b83c5e3-404e-44ea-8013-122b6914a",
                Name = "Admin"
            },
            new Role()
            {
                Id = "0b83c5e3-404e-44ea-8013-12253fa",
                Name = "User"
            }
        };

        public static readonly IList<string> RolesString = new List<string>()
        {
           "Admin","Blog"
        };

        public static readonly RoleEditDto roleEditDto = new RoleEditDto
        {
            RoleNames =new [] {"User"} 
        };

        public static readonly Setting settingForUpload = new Setting()
        {
            CloudinaryCloudName = "12",
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
            Password = "password",
            Name = "کیوان",
            PhoneNumber = "15486523"
        };

        public static readonly UserForRegisterDto userForRegisterDto_Fail_Exist = new UserForRegisterDto()
        {
            UserName = "kathybrown@barkarama.com",
            Password = "password",
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

        public static readonly TokenRequestDto useForLoginDto_Success_password = new TokenRequestDto()
        {
            GrantType = "password",
            UserName = "keyvan@madpay.com",
            Password = "password",
            IsRemember = true
        };
        public static readonly TokenRequestDto useForLoginDto_Success_refreshToken = new TokenRequestDto()
        {
            GrantType = "refresh_token",
            UserName = "keyvan@madpay.com",
            RefreshToken = "",
            IsRemember = true
        };

        public static readonly TokenRequestDto useForLoginDto_Fail = new TokenRequestDto()
        {
            GrantType = "password",
            UserName = "00@000.com",
            Password = "password",
            IsRemember = true
        };

        public static readonly TokenRequestDto useForLoginDto_Fail_ModelState = new TokenRequestDto()
        {
            
            UserName = string.Empty,
            GrantType = string.Empty
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
        { };

        public static readonly FileUploadedDto fileUploadedDto_Success = new FileUploadedDto()
        {
            Status = true,
            LocalUploaded = true,
            Message = "با موفقیت در لوکال آپلود شد",
            PublicId = "0",
            Url = "wwwroot/Files/Pic/Profile/"
        };

        public static readonly FileUploadedDto fileUploadedDto_Fail_WrongFile = new FileUploadedDto()
        {
            Status = false,
            Message = "فایلی برای اپلود یافت نشد"
        };

    }
}

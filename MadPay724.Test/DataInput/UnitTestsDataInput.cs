using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;

namespace MadPay724.Test.DataInput
{
    public static class UnitTestsDataInput
    {
        public static readonly string unToken ="";
        public static readonly string aToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2MzQ1NjA3NywiZXhwIjoxNTYzNTQyNDc3LCJpYXQiOjE1NjM0NTYwNzd9.q4iLi5Zc6in0QWd6az-sgqHHn27Q7w9pP9znjGYKlID0mOBIC2kx-3njB5z3NDoecjwpw4mtywFfPMJC3QdSNQ";



        public static readonly string userLogedInUsername = "haysmathis@barkarama.com";
        public static readonly string userLogedInPassword = "123789";
        public static readonly string userLogedInId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
        public static readonly string userAnOtherId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
        public static readonly string userLogedInPhotoId = "e97fd389-fb3d-4ea2-929d-435f5ecdc159";

        public static readonly UserForRegisterDto userForRegisterDto = new UserForRegisterDto()
        {
            UserName = "asasa78sasas@barkarama.com",
            Password = "123789",
            Name = "کیوان",
            PhoneNumber = "15486523"
        };
        public static readonly UserForRegisterDto userForRegisterDto_Fail_Exist = new UserForRegisterDto()
        {
            UserName = "haysmathis@barkarama.com",
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
            UserName = "haysmathis@barkarama.com",
            Password = "123789",
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
            City = "لورم ایپسوم متن ساختگی با تولید سادگلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی دری نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی در."

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
    }
}

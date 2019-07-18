using MadPay724.Common.ReturnMessages;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Test.IntegrationTests.Providers;
using Xunit;

namespace MadPay724.Test.IntegrationTests.ControllersTests
{
    public class UsersControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        private readonly string _unToken;
        private readonly string _aToken;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
            _unToken = "";
            //0d47394e-672f-4db7-898c-bfd8f32e2af7
            //haysmathis@barkarama.com
            //123789
            _aToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2MjkzNDI0NywiZXhwIjoxNTYzMDIwNjQ3LCJpYXQiOjE1NjI5MzQyNDd9.ZaWbyiXyJk3qIgEci_HMi1h3tiMeUzsP3h8H-7f8f31viUsD6PkN18lYa88g5_NVUxoX7PAXuZvH2exFy7boWA";
        }

        #region GetUserTests
        [Fact]
        public async Task GetUser_Success_GetUserHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimSelfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = "/site/admin/Users/" + userHimSelfId;
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.GetAsync(request);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task GetUser_Fail_GetAnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = "/site/admin/Users/" + anOtherUserId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.GetAsync(request);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        #endregion

        #region UpdateUserTests
        [Fact]
        public async Task UpdateUser_Success_UpdateUserHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = new
            {
                Url = "/site/admin/Users/" + userHimselfId,
                Body = new
                {
                    Name = "علی حسینی",
                    PhoneNumber = "string",
                    Address = "string",
                    Gender = true,
                    City = "string"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_Fail_UpdateAnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/" + anOtherUserId,
                Body = new
                {
                    Name = "علی حسینی",
                    PhoneNumber = "string",
                    Address = "string",
                    Gender = true,
                    City = "string"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/" + userHimselfId,
                Body = new UserForUpdateDto
                {
                    Name = string.Empty,
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    City = "لورم ایپسوم متن ساختگی با تولید سادگلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی دری نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی در."
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            var controller = new ModelStateController();


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            controller.ValidateModelState(request.Body);
            var modelState = controller.ModelState;

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.False(modelState.IsValid);
            Assert.Equal(4, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("Name") && modelState.Keys.Contains("PhoneNumber")
                && modelState.Keys.Contains("Address") && modelState.Keys.Contains("City"));

        }
        #endregion

        #region ChangeUserPasswordTests
        [Fact]
        public async Task ChangeUserPassword_Success_Himself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = new
            {
                Url = "/site/admin/Users/ChangeUserPassword/" + userHimselfId,
                Body = new PasswordForChangeDto
                {
                    OldPassword = "123789",
                    NewPassword = "123789"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPassword_Fail_AnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/ChangeUserPassword/" + anOtherUserId,
                Body = new PasswordForChangeDto
                {
                    OldPassword = "123789",
                    NewPassword = "123789"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPassword_Fail_Himself_WrongOldPassword()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = new
            {
                Url = "/site/admin/Users/ChangeUserPassword/" + userHimselfId,
                Body = new PasswordForChangeDto
                {
                    OldPassword = "123789654645",
                    NewPassword = "123789"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();
            var valueObj = JsonConvert.DeserializeObject<ReturnMessage>(value);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(valueObj.status);
            Assert.Equal("پسورد قبلی اشتباه میباشد", valueObj.message);


        }
        [Fact]
        public async Task ChangeUserPassword_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/ChangeUserPassword/" + userHimselfId,
                Body = new PasswordForChangeDto
                {
                    OldPassword = string.Empty,
                    NewPassword = string.Empty
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _aToken);

            var controller = new ModelStateController();


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            controller.ValidateModelState(request.Body);
            var modelState = controller.ModelState;

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.False(modelState.IsValid);
            Assert.Equal(2, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("OldPassword") && modelState.Keys.Contains("NewPassword"));

        }
        #endregion
    }
}

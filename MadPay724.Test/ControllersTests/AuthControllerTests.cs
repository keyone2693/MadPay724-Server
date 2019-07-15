using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation;
using MadPay724.Test.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Common.ReturnMessages;
using Microsoft.AspNetCore.Mvc.Versioning;
using Xunit;

namespace MadPay724.Test.ControllersTests
{
    public class AuthControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        private readonly string _UnToken;
        private readonly string _AToken;
        public AuthControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
            _UnToken = "";
            //0d47394e-672f-4db7-898c-bfd8f32e2af7
            //haysmathis@barkarama.com
            //123789
            _AToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2MjkzNDI0NywiZXhwIjoxNTYzMDIwNjQ3LCJpYXQiOjE1NjI5MzQyNDd9.ZaWbyiXyJk3qIgEci_HMi1h3tiMeUzsP3h8H-7f8f31viUsD6PkN18lYa88g5_NVUxoX7PAXuZvH2exFy7boWA";
        }

        #region loginTests
        [Fact]
        public async Task Login_Success_UserLogin()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/login";
            var model = new UseForLoginDto()
            {
                UserName = "haysmathis@barkarama.com",
                Password = "123789",
                IsRemember = true
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request, ContentHelper.GetStringContent(model));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Login_Fail_UserLogin()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/login";
            var model = new UseForLoginDto()
            {
                UserName = "00@000.com",
                Password = "0000",
                IsRemember = true
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request, ContentHelper.GetStringContent(model));
            var actual = await response.Content.ReadAsAsync<string>();
            //Assert-------------------------------------------------------------------------------------------------------------------------------

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("کاربری با این یوزر و پس وجود ندارد", actual);
        }
        [Fact]
        public async Task Login_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = new
            {
                Url = "/site/admin/auth/login",
                Body = new UseForLoginDto
                {
                    UserName = string.Empty,
                    Password = string.Empty
                }
            };
            var controller = new ModelStateControllerTests();
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            controller.ValidateModelState(request.Body);
            var modelState = controller.ModelState;
            //Assert-------------------------------------------------------------------------------------------------------------------------------


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.False(modelState.IsValid);
            Assert.Equal(2, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("UserName") && modelState.Keys.Contains("Password"));

            //Assert


        }
        #endregion

        #region registerTests
        [Fact]
        public async Task Register_Success_UserRegister()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/register";
            var model = new UserForRegisterDto()
            {
                UserName = "asasasasas@barkarama.com",
                Password = "123789",
                Name  = "کیوان",
                PhoneNumber =  "15486523"
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request, ContentHelper.GetStringContent(model));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        [Fact]
        public async Task Register_Fail_UserExist()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/register";
            var model = new UserForRegisterDto()
            {
                UserName = "haysmathis@barkarama.com",
                Password = "123789",
                Name = "کیوان",
                PhoneNumber = "15486523"
            };
            var expected = new ReturnMessage()
            {
                status = false,
                title = "خطا",
                message = "نام کاربری وجود دارد"
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request, ContentHelper.GetStringContent(model));
            var actual = await response.Content.ReadAsAsync<ReturnMessage>();
            //Assert-------------------------------------------------------------------------------------------------------------------------------

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.IsType<ReturnMessage>(actual);

            Assert.False(expected.status);
            Assert.Equal(expected.title, actual.title);
            Assert.Equal(expected.message, actual.message);

            
        }
        [Fact]
        public async Task Register_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = new
            {
                Url = "/site/admin/auth/register",
                Body = new UserForRegisterDto
                {
                    UserName = string.Empty,
                    Password = string.Empty,
                    Name = string.Empty,
                    PhoneNumber = string.Empty
                }
            };
            var controller = new ModelStateControllerTests();
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            controller.ValidateModelState(request.Body);
            var modelState = controller.ModelState;
            //Assert-------------------------------------------------------------------------------------------------------------------------------


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.False(modelState.IsValid);
            Assert.Equal(4, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("UserName") && modelState.Keys.Contains("Password")
                        && modelState.Keys.Contains("Name") && modelState.Keys.Contains("PhoneNumber"));

            //Assert


        }
        #endregion

    }
}

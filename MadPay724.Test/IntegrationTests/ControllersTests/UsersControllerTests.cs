using MadPay724.Data.Dtos.Site.Panel.Users;
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
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using Xunit;
using MadPay724.Common.ErrorAndMessage;

namespace MadPay724.Test.IntegrationTests.ControllersTests
{
    public class UsersControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
        }

        #region GetUserTests
        [Fact]
        public async Task GetUser_Success_GetUserHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimSelfId = UnitTestsDataInput.userLogedInId;
            var request = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/" + userHimSelfId;
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

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
            string anOtherUserId = UnitTestsDataInput.userAnOtherId;
            var request = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/" + anOtherUserId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

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
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/" + userHimselfId,
                Body = UnitTestsDataInput.userForUpdateDto
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

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
            string anOtherUserId = UnitTestsDataInput.userAnOtherId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/" + anOtherUserId,
                Body = UnitTestsDataInput.userForUpdateDto
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/" + userHimselfId,
                Body = UnitTestsDataInput.userForUpdateDto_Fail_ModelState
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region ChangeUserPasswordTests
        [Fact]
        public async Task ChangeUserPassword_Success_Himself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/ChangeUserPassword/" + userHimselfId,
                Body = UnitTestsDataInput.passwordForChangeDto
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

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
            string anOtherUserId = UnitTestsDataInput.userAnOtherId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/ChangeUserPassword/" + anOtherUserId,
                Body = UnitTestsDataInput.passwordForChangeDto
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPassword_Fail_Himself_WrongOldPassword()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/ChangeUserPassword/" + userHimselfId,
                Body = UnitTestsDataInput.passwordForChangeDto_Fail
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

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
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/Users/ChangeUserPassword/" + userHimselfId,
                Body = UnitTestsDataInput.passwordForChangeDto_Fail_ModelState
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

           

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

          

        }
        #endregion
    }
}

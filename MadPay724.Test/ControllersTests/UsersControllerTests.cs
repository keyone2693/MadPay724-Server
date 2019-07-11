using FluentAssertions;
using MadPay724.Presentation;
using MadPay724.Test.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace MadPay724.Test.ControllersTests
{
    public class UsersControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        private readonly string _UnToken;
        private readonly string _AToken;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
            _UnToken = "";
            //0d47394e-672f-4db7-898c-bfd8f32e2af7
            //haysmathis@barkarama.com
            //123789
            _AToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2Mjg0NTM2NCwiZXhwIjoxNTYyOTMxNzY0LCJpYXQiOjE1NjI4NDUzNjR9.44SJQ97Zi_5lbNlGtp92xsjb6T0SrCWk2X8uCSgtCHN7BdbtsPJjX8T2GtcxlQ3H8x-JCaCJ9tBaSV_VhA7M-Q";
        }
        #region GetUserTests
        [Fact]
        public async void GetUser_CantGetAnOtherUser()
        {
            // Arrange
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = "/site/admin/Users/" + anOtherUserId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async void GetUser_CanGetUserHimself()
        {
            // Arrange
            string userHimSelfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = "/site/admin/Users/" + userHimSelfId;
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region UpdateUserTests
        [Fact]
        public async void UpdateUser_CantUpdateAnOtherUser()
        {
            // Arrange
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
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async void UpdateUser_CanUpdateUserHimself()
        {
            // Arrange
            string anOtherUserId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
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
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        #endregion

        #region ChangeUserPasswordTests

        #endregion
    }
}

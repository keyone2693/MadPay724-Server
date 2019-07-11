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
    public class UsersControllerTests: IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        private readonly string _UnToken;
        private readonly string _AToken;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
            _UnToken = "";
            _AToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2Mjg0NTM2NCwiZXhwIjoxNTYyOTMxNzY0LCJpYXQiOjE1NjI4NDUzNjR9.44SJQ97Zi_5lbNlGtp92xsjb6T0SrCWk2X8uCSgtCHN7BdbtsPJjX8T2GtcxlQ3H8x-JCaCJ9tBaSV_VhA7M-Q";
        }
        [Fact]
        public async void GetUsers_Unauthorized_User_CantGetUsers()
        {
            // Arrange
            var request = "/site/admin/Users";
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _UnToken);

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async void GetUsers_Authorized_Can_GetUsers()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", _AToken);
            var request = "/site/admin/Users";

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

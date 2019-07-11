using MadPay724.Presentation;
using MadPay724.Test.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace MadPay724.Test.ControllersTests
{
    public class UsersControllerTests: IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
        }
        [Fact]
        public async void GetUsers_CantGetUsers()
        {
            // Arrange
            var request = "/site/admin/Users";

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        //Failed
        //Succeeded
    }
}

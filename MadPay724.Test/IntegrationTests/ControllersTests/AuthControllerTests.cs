using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using Xunit;

namespace MadPay724.Test.IntegrationTests.ControllersTests
{
    public class AuthControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
        }

        #region loginTests
        [Fact]
        public async Task Login_Success_UserLogin()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/login";
            var model = UnitTestsDataInput.useForLoginDto_Success;
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
            var model = UnitTestsDataInput.useForLoginDto_Fail;
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
                Body = UnitTestsDataInput.useForLoginDto_Fail_ModelState
            };
            
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            
            //Assert-------------------------------------------------------------------------------------------------------------------------------


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

           

            //Assert


        }
        #endregion

        #region registerTests
        [Fact]
        public async Task Register_Success_UserRegister()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var request = "/site/admin/auth/register";
            var model = UnitTestsDataInput.userForRegisterDto;
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
            var model = UnitTestsDataInput.userForRegisterDto_Fail_Exist;
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
                Body = UnitTestsDataInput.userForRegisterDto_Fail_ModelState
            };
            var controller = new ModelStateController();
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

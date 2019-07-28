using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using Xunit;

namespace MadPay724.Test.IntegrationTests.ControllersTests
{

    public class PhotosControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        public PhotosControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
        }

        #region GetPhotoTests
        [Fact]
        public async Task GetPhoto_Success_Himself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            string userPhotoId = UnitTestsDataInput.userLogedInPhotoId;

            var request = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + userHimselfId + "/photos/" + userPhotoId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.GetAsync(request);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetPhoto_Fail_Himself_SeeAnOtherOnePhoto()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;
            string userPhotoId = UnitTestsDataInput.userLogedInPhotoId;

            var request = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + userHimselfId + "/photos/" + userPhotoId;

            _client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.GetAsync(request);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetPhoto_Fail_AnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userAnOtherId;
            string userPhotoId = UnitTestsDataInput.userLogedInPhotoId;

            var request = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + userHimselfId + "/photos/" + userPhotoId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.GetAsync(request);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        #endregion



        #region ChangeUserPhotoTests
        [Fact]
        public async Task ChangeUserPhoto_Success_Himself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var fileMock = new Mock<IFormFile>();
            var fileName = "0d47394e-672f-4db7-898c-bfd8f32e2af7.jpg";

            string userHimselfId = UnitTestsDataInput.userLogedInId;


            Bitmap image = new Bitmap(50, 50);
            Graphics imageData = Graphics.FromImage(image);
            imageData.DrawLine(new Pen(Color.Red), 0, 0, 50, 50);
            MemoryStream memoryStream = new MemoryStream();
            byte[] bitmapData;
            using (memoryStream)
            {
                image.Save(memoryStream, ImageFormat.Bmp);
                bitmapData = memoryStream.ToArray();
            }

            var ms = new MemoryStream(bitmapData);
            var writer = new StreamWriter(ms);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            byte[] data;
            using (var br = new BinaryReader(fileMock.Object.OpenReadStream()))
                data = br.ReadBytes((int)fileMock.Object.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "File", fileMock.Object.FileName);

            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + userHimselfId + "/photos",
                Body = UnitTestsDataInput.photoForProfileDto
            };

            multiContent.Add(ContentHelper.GetStringContent(request.Body));

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, multiContent);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPhoto_Fail_WorngFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string userHimselfId = UnitTestsDataInput.userLogedInId;

            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")),
                0, 0, "Data", "0d47394e-672f-4db7-898c-bfd8f32e2af7.png");

            byte[] data;
            using (var br = new BinaryReader(file.OpenReadStream()))
                data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(bytes, "File", file.FileName);

            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + userHimselfId + "/photos",
                Body = UnitTestsDataInput.photoForProfileDto
            };

            multiContent.Add(ContentHelper.GetStringContent(request.Body));

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, multiContent);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPhoto_Fail_AnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            string anOtherUserId = UnitTestsDataInput.userAnOtherId;

            var request = new
            {
                Url = UnitTestsDataInput.baseRouteV1 + "site/panel/users/" + anOtherUserId + "/photos",
                Body = UnitTestsDataInput.photoForProfileDto
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", UnitTestsDataInput.aToken);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

    }
}

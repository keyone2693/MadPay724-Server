using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Site.Home;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MadPay724.Presentation.Controllers.Site.V1.Site
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Home")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;
        private readonly IUtilities _utilities;
        //private readonly SignInManager<Data.Models.User> _signInManager;
        private ApiReturn<string> errorModel;


        public HomeController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper, ILogger<HomeController> logger, IUtilities utilities)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            errorModel = new ApiReturn<string>
            {
                Status = false,
                Message = "",
                Result = null
            };
        }

        [HttpGet(ApiV1Routes.Home.GetHomeData)]
        [ProducesResponseType(typeof(ApiReturn<HomeDataReturnDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHomeData()
        {
            var model = new ApiReturn<HomeDataReturnDto>
            {
                Result = new HomeDataReturnDto()
            };

            var feedBackJson = System.IO.File.ReadAllText("wwwroot/Files/Json/Home/FeedBack.json");
            model.Result.FeedBacks = JsonConvert.DeserializeObject<List<FeedBackDto>>(feedBackJson);

            var serviceStatJson = System.IO.File.ReadAllText("wwwroot/Files/Json/Home/ServiceStat.json"); ;
            model.Result.ServiceStat = JsonConvert.DeserializeObject<ServiceStatDto>(serviceStatJson);

            model.Result.Customers = new List<CustomerDto>();
            var dir = new DirectoryInfo("wwwroot/Files/CustomerImg");

            if (dir.Exists)
            {
                var files = dir.GetFiles().ToList().Take(5);
                foreach (var file in files)
                {
                    model.Result.Customers.Add(new CustomerDto
                    {
                        Name = "...",
                        Url = $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}" +
                        "/wwwroot/Files/CustomerImg/" + Path.GetFileName(file.FullName),
                    });
                }
            }

            var blogsFromRepo = await _db.BlogRepository
               .GetManyAsync(p => p.Status, s => s.OrderByDescending(x => x.DateModified), "User,BlogGroup");

            var blogs = new List<BlogForReturnDto>();

            foreach (var item in blogsFromRepo)
            {
                blogs.Add(_mapper.Map<BlogForReturnDto>(item));
            }




            model.Result.LastBlogs = blogs;

            model.Status = true;
            model.Message = "اطلاعات با موفقیت بارگزاری شد";
            return Ok(model);
        }
    }
}
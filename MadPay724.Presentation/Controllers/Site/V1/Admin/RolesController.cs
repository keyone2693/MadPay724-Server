using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Roles;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly Main_MadPayDbContext _dbMad;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;
        private readonly UserManager<Data.Models.MainDB.User> _userManager;



        public RolesController(IUnitOfWork<Main_MadPayDbContext> dbContext, Main_MadPayDbContext dbMad
            , IMapper mapper,
            ILogger<RolesController> logger, UserManager<Data.Models.MainDB.User> userManager)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _dbMad = dbMad;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost(ApiV1Routes.AdminRoles.EditRoles)]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles ??= new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                return BadRequest("خطا در اضافه کردن نقش ها");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                return BadRequest("خطا در پاک کردن نقش ها");
            }

            return Ok(await _userManager.GetRolesAsync(user));

        }
    }
}
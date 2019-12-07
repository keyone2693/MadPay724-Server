using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Roles;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.User.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly MadpayDbContext _dbMad;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUsersController> _logger;
        private readonly UserManager<Data.Models.MainDB.User> _userManager;



        public AdminUsersController(IUnitOfWork<MadpayDbContext> dbContext, MadpayDbContext dbMad,
            IMapper mapper,
            ILogger<AdminUsersController> logger, UserManager<Data.Models.MainDB.User> userManager)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _dbMad = dbMad;
            _userManager = userManager;
        }
        //[AllowAnonymous]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminUsers.GetUsers)]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetUsers()
        {
            var users =await (from user in _dbMad.Users
                              orderby user.UserName
                         select new
                         {
                             Id = user.Id,
                             UserName = user.UserName,
                             Roles = (from userRole in user.UserRoles
                                      join role in _dbMad.Roles
                                          on userRole.RoleId
                                          equals role.Id
                                      select role.Name)
                         }).ToListAsync();
           //await _db.UserRepository.GetManyAsync(null, null, "Photos,BankCards");
            // var usersToReturn = _mapper.Map<IEnumerable<UserFroListDto>>(users);

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost(ApiV1Routes.AdminUsers.EditRoles)]
        public async Task<IActionResult> EditRoles(string userName,RoleEditDto roleEditDto)
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
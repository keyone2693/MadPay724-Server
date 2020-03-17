using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Role;
using MadPay724.Data.Dtos.Site.Panel.Roles;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
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
        [HttpGet(ApiV1Routes.AdminRoles.GetUserRoles)]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userRoles = await _userManager.GetRolesAsync(user);


            var roles = new List<RolesForReturnDto>
            {
                new RolesForReturnDto
                {UserId = userId, Value = "Admin", Has = false},
                new RolesForReturnDto
                {UserId = userId, Value = "Accountant", Has = false},
                new RolesForReturnDto
                {UserId = userId, Value = "AdminBlog", Has = false},
                new RolesForReturnDto
                {UserId = userId, Value = "Blog", Has = false},
                new RolesForReturnDto
                {UserId = userId, Value = "User", Has = false}
            };




            foreach (var item in userRoles)
            {
                roles.Single(p => p.Value == item).Has = true;
            }

            return Ok(roles);

        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPatch(ApiV1Routes.AdminRoles.ChangeRoles)]
        public async Task<IActionResult> ChangeRoles(string userId, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userRoles = await _userManager.GetRolesAsync(user);

            if(userRoles.Any(p=>p == roleEditDto.Value))
            {
                if (roleEditDto.Check)
                {
                    return NoContent();
                }
                else
                {
                    var result = await _userManager
                         .RemoveFromRoleAsync(user, roleEditDto.Value);
                    if (!result.Succeeded)
                    {
                        return BadRequest("خطا در پاک کردن نقش ");
                    }
                    else
                    {
                        return NoContent();
                    }
                }
            }
            else
            {
                if (!roleEditDto.Check)
                {
                    return NoContent();
                }
                else
                {
                    var result = await _userManager
                        .AddToRoleAsync(user, roleEditDto.Value);
                    if (!result.Succeeded)
                    {
                        return BadRequest("خطا در اضافه کردن نقش ");
                    }
                    else
                    {
                        return NoContent();
                    }
                }
            }            
        }
    }
}
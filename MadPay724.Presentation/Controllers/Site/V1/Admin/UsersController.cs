using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Common.Helpers.Utilities.Pagination;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Roles;
using MadPay724.Data.Dtos.Site.Panel.Users;
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
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly Main_MadPayDbContext _dbMad;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<Data.Models.MainDB.User> _userManager;



        public UsersController(IUnitOfWork<Main_MadPayDbContext> dbContext, Main_MadPayDbContext dbMad
            ,IMapper mapper,
            ILogger<UsersController> logger, UserManager<Data.Models.MainDB.User> userManager)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _dbMad = dbMad;
        }
        //[AllowAnonymous]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminUsers.GetUsers)]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetUsers([FromQuery]PaginationDto paginationDto)
        {
            var usersFromRepo = await _db.UserRepository.GetAllPagedListAsync(
                 paginationDto,
                 paginationDto.Filter.ToUserExpression(true),
                 paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                 "");

            Response.AddPagination(usersFromRepo.CurrentPage, usersFromRepo.PageSize,
                usersFromRepo.TotalCount, usersFromRepo.TotalPage);

            var users = new List<UserForAccountantDto>();

            foreach (var item in usersFromRepo)
            {
                users.Add(_mapper.Map<UserForAccountantDto>(item));
            }

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
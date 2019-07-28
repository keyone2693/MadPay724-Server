using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.ION;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.User.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IUserService _userService;
        private readonly ILogger<AdminUsersController> _logger;


        public AdminUsersController(IUnitOfWork<MadpayDbContext> dbContext, MadpayDbContext dbMad,
            IMapper mapper,
            IUserService userService, ILogger<AdminUsersController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _userService = userService;
            _logger = logger;
            _dbMad = dbMad;
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

    }
}
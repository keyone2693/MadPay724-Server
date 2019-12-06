using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class AccountantUsersController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly MadpayDbContext _dbMad;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountantUsersController> _logger;
        private readonly UserManager<Data.Models.User> _userManager;



        public AccountantUsersController(IUnitOfWork<MadpayDbContext> dbContext, MadpayDbContext dbMad,
            IMapper mapper,
            ILogger<AccountantUsersController> logger, UserManager<Data.Models.User> userManager)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _dbMad = dbMad;
            _userManager = userManager;
        }
        //[AllowAnonymous]
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetUsers)]
        public async Task<IActionResult> GetUsers(string id, [FromQuery]PaginationDto paginationDto)
        {

            var usersFromRepo = await _db.UserRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToUserExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "Wallets,Photos");//,BankCards
            
            Response.AddPagination(usersFromRepo.CurrentPage, usersFromRepo.PageSize,
                usersFromRepo.TotalCount, usersFromRepo.TotalPage);

            var users = new List<UserForAccountantDto>();

            foreach (var item in usersFromRepo)
            {
                users.Add(_mapper.Map<UserForAccountantDto>(item));
            }

            return Ok(users);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ReturnMessages;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "Site")]
    [Route("site/admin/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        public UsersController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper)
        {
            _db = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.UserRepository.GetManyAsync(null, null, "Photos,BankCards");

            var usersToReturn = _mapper.Map<IEnumerable<UserFroListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _db.UserRepository.GetManyAsync(p => p.Id == id, null, "Photos");
            var userToReturn = _mapper.Map<UserForDetailedDto>(user.SingleOrDefault());
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserForUpdateDto userForUpdateDto)
        {
            if(id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Unauthorized("شما اجازه ویرایش این کاربر را ندارید");
            }

            var userFromRepo = await _db.UserRepository.GetByIdAsync(id);

            _mapper.Map(userForUpdateDto, userFromRepo);
            _db.UserRepository.Update(userFromRepo);

            if(await _db.SaveAsync())
            {
                return NoContent();
            }
            else{
                return Unauthorized(new ReturnMessage()
                {
                    status = false,
                    title = "خطا",
                    message = "خطا در ویرایش"
                });
            }


        }

        //[Route("GetProfileUser/{id}")]
        //[HttpGet]
        //public async Task<IActionResult> GetProfileUser(string id)
        //{
        //    if (User.FindFirst(ClaimTypes.NameIdentifier).Value == id)
        //    {
        //        var user = await _db.UserRepository.GetManyAsync(p => p.Id == id, null, "Photos");
        //        var userToReturn = _mapper.Map<UserForDetailedDto>(user.SingleOrDefault());
        //        return Ok(userToReturn);
        //    }
        //    else
        //    {
        //        return Unauthorized("شما به این اطلاعات دسترسی ندارید");
        //    }
        //}
    }
}
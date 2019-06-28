using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Admin.Auth.Interface
{
    public interface IUserService
    {
        Task<User> GetUserForPassChange(string id, string password);
        Task<bool> UpdateUserPass(User user, string newPassword);
    }
}

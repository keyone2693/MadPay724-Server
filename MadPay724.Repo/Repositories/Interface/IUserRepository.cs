using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Repo.Repositories.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> UserExists(string username);
    }
}

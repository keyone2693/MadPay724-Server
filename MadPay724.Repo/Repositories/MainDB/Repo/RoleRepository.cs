using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.MainDB.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.MainDB.Repo
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly DbContext _db;
        public RoleRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Main_MadPayDbContext)_db;
        }
    }
}

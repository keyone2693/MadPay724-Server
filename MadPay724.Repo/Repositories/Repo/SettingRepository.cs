using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Repo.Repositories.Repo
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        private readonly DbContext _db;
        public SettingRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (MadpayDbContext)_db;
        }
    }
}

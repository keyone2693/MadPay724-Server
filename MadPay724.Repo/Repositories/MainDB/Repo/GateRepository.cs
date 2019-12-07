using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB.UserModel;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.MainDB.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Repo.Repositories.MainDB.Repo
{
    public class GateRepository : Repository<Gate>, IGateRepository
    {
        private readonly DbContext _db;
        public GateRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Main_MadpayDbContext)_db;
        }
    }
}

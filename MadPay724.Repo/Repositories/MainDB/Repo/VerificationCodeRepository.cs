using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.MainDB.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Repo.Repositories.MainDB.Repo
{
    public class VerificationCodeRepository : Repository<VerificationCode>, IVerificationCodeRepository
    {
        private readonly DbContext _db;
        public VerificationCodeRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Main_MadPayDbContext)dbContext;
        }
    }
}

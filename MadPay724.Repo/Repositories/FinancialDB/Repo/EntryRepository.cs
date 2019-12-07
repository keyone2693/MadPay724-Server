using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.FinancialDB.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Repo.Repositories.FinancialDB.Repo
{
    public class EntryRepository : Repository<Entry>, IEntryRepository
    {
        private readonly DbContext _db;
        public EntryRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Financial_MadPayDbContext)_db;
        }
    }
}

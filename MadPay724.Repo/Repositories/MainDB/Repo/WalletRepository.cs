using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.MainDB.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.MainDB.Repo
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        private readonly DbContext _db;
        public WalletRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Main_MadPayDbContext)_db;
        }

        public async Task<long> GetLastWalletCodeAsync()
        {
            return (await GetManyAsync(null,p=>p.OrderByDescending(s=>s.Code),"")).First().Code;
        }

        public async Task<bool> WalletCodeExistAsync(long code)
        {
            return (await GetManyAsync(p => p.Code == code, null, "")).Any();
        }

        public async Task<int> WalletCountAsync(string userId)
        {
            return (await GetManyAsync(p => p.Id == userId, null, "")).Count();
        }
    }
}

using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB.UserModel;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.Repo
{
  public  class EasyPayRepository : Repository<EasyPay>, IEasyPayRepository
    {
        private readonly DbContext _db;
        public EasyPayRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (MadpayDbContext)_db;
        }
    }
}

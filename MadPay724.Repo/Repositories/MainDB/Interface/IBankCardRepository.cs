using System.Threading.Tasks;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;

namespace MadPay724.Repo.Repositories.MainDB.Interface
{
    public interface IBankCardRepository : IRepository<BankCard>
    {
        Task<int> BankCardCountAsync(string userId);
    }
}

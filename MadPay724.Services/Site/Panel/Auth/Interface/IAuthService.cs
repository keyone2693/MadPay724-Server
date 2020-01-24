using MadPay724.Data.Models.MainDB;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Panel.Auth.Interface
{
    public interface IAuthService
    {
        Task<Data.Models.MainDB.User> RegisterAsync(Data.Models.MainDB.User user,Photo photo, string password);
        Task<Data.Models.MainDB.User> LoginAsync(string username, string password);

        Task<bool> AddUserPreNeededAsync(Photo photo,Notification notify,Data.Models.MainDB.Wallet walletMain,Data.Models.MainDB.Wallet walletSms );

    }
}

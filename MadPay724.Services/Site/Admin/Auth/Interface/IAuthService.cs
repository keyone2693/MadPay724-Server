using MadPay724.Data.Models;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Admin.Auth.Interface
{
    public interface IAuthService
    {
        Task<Data.Models.User> RegisterAsync(Data.Models.User user,Photo photo, string password);
        Task<Data.Models.User> LoginAsync(string username, string password);

        Task<bool> AddUserPreNeededAsync(Data.Models.Photo photo,Notification notify,Wallet walletMain,Wallet walletSms );

    }
}

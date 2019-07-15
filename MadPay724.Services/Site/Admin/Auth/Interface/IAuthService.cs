using MadPay724.Data.Models;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Admin.Auth.Interface
{
    public interface IAuthService
    {
        Task<Data.Models.User> Register(Data.Models.User user,Photo photo, string password);
        Task<Data.Models.User> Login(string username, string password);
    }
}

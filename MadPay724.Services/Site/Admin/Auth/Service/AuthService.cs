using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Interface;

namespace MadPay724.Services.Site.Admin.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUtilities _utilities;

        public AuthService(IUnitOfWork<Main_MadPayDbContext> dbContext, IUtilities utilities)
        {
            _db = dbContext;
            _utilities = utilities;
        }
        public async Task<bool> AddUserPreNeededAsync(Photo photo,Notification notify, Data.Models.MainDB.Wallet walletMain, Data.Models.MainDB.Wallet walletSms)
        {
            if(walletMain !=null)
                await _db.WalletRepository.InsertAsync(walletMain);
            if (walletSms != null)
                await _db.WalletRepository.InsertAsync(walletSms);

            await _db.PhotoRepository.InsertAsync(photo);
            await _db.NotificationRepository.InsertAsync(notify);
            if (await _db.SaveAsync())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<Data.Models.MainDB.User> LoginAsync(string username, string password)
        {
            var users = await _db.UserRepository.GetManyAsync(p => p.UserName == username, null, "Photos");
            var user = users.SingleOrDefault();

            if (user == null)
            {
                return null;
            }

            //if (!_utilities.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //    return null;


            return user;
        }

        public async Task<Data.Models.MainDB.User> RegisterAsync(Data.Models.MainDB.User user, Photo photo, string password)
        {
            byte[] passwordHash, passwordSalt;
            _utilities.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            await _db.UserRepository.InsertAsync(user);
            await _db.PhotoRepository.InsertAsync(photo);
            if (await _db.SaveAsync())
            {
                return user;
            }
            else
            {
                return null;
            }
        }




    }
}

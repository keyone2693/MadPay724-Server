using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Services.Site.Panel.User.Interface;
using Microsoft.AspNetCore.Identity;

namespace MadPay724.Services.Site.Panel.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUtilities _utilities;
        private readonly UserManager<Data.Models.MainDB.User> _userManager;

        public UserService(IUnitOfWork<Main_MadPayDbContext> dbContext,IUtilities utilities,
             UserManager<Data.Models.MainDB.User> userManager)
        {
            _db = dbContext;
            _utilities = utilities;
            _userManager= userManager;
        }

 

        public async Task<Data.Models.MainDB.User> GetUserForPassChange(string id, string password)
        {

            var user = await _db.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            //if (!_utilities.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //    return null;


            return user;
        }

        public async Task<bool> UpdateUserPass(Data.Models.MainDB.User user, string newPassword)
        {
            byte[] passwordHash, passwordSalt;
            _utilities.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);


            //user.PasswordHash = passwordHash.ToString();

            //_db.UserRepository.Update(user);

            return result.Succeeded;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Models;

namespace MadPay724.Common.Helpers.Interface
{
    public interface IUtilities
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
         string GenerateJwtToken(User user, bool isRemember);
    }
}

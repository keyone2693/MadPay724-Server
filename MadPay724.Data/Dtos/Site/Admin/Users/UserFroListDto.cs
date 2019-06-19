using MadPay724.Data.Dtos.Site.Admin.BankCards;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserFroListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public ICollection<PhotoForUserDetailedDto> Photos { get; set; }
        public ICollection<BankCardForUserDetailedDto> BankCards { get; set; }
    }
}

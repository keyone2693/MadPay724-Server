using MadPay724.Data.Dtos.Site.Admin.BankCards;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Common;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserFroListDto: BaseDto
    {

        public Link UpdateUser { get; set; }
        public Link ChangeUserPassword { get; set; }


        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public ICollection<PhotoForUserDetailedDto> Photos { get; set; }
        public ICollection<BankCardForUserDetailedDto> BankCards { get; set; }
    }
}

using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.Photos;
using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Common.ION;

namespace MadPay724.Data.Dtos.Site.Panel.Users
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

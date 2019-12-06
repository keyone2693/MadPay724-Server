using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Users
{
   public class UserForAccountantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
        public int InventorySum { get; set; }
        public int InterMoneySum { get; set; }
        public int ExitMoneySum { get; set; }
        public int OnExitMoneySum { get; set; }
    }
}

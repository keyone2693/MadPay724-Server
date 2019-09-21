using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.EasyPay
{
   public class EasyPayForReturnDto
    {
        
        public string Id { get; set; }
        public string WalletGateId { get; set; }
        public bool IsWallet { get; set; }
        public string Name { get; set; }
                public int Price { get; set; }
                public string Text { get; set; }
        
        public bool IsCoupon { get; set; }
        
        public bool IsUserEmail { get; set; }
        
        public bool IsUserName { get; set; }
        
        public bool IsUserPhone { get; set; }
        
        public bool IsUserText { get; set; }
        
        public bool IsUserEmailRequired { get; set; }
        
        public bool IsUserNameRequired { get; set; }
        
        public bool IsUserPhoneRequired { get; set; }
        
        public bool IsUserTextRequired { get; set; }
        
        public string UserEmailExplain { get; set; }
        
        public string UserNameExplain { get; set; }
        
        public string UserPhoneExplain { get; set; }
        
        public string UserTextExplain { get; set; }

        public bool IsCountLimit { get; set; }
        public int CountLimit { get; set; }
        public string ReturnSuccess { get; set; }
        public string ReturnFail { get; set; }
    }
}

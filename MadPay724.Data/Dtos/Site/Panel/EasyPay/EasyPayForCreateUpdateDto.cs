using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.EasyPay
{
    public class EasyPayForCreateUpdateDto
    {
        [Required]
        public string WalletGateId { get; set; }
        [Required]
        public bool IsWallet { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        [StringLength(250, MinimumLength = 0)]
        public string Text { get; set; }
        [Required]
        public bool IsCoupon { get; set; }
        [Required]
        public bool IsUserEmail { get; set; }
        [Required]
        public bool IsUserName { get; set; }
        [Required]
        public bool IsUserPhone { get; set; }
        [Required]
        public bool IsUserText { get; set; }
        [Required]
        public bool IsUserEmailRequired { get; set; }
        [Required]
        public bool IsUserNameRequired { get; set; }
        [Required]
        public bool IsUserPhoneRequired { get; set; }
        [Required]
        public bool IsUserTextRequired { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0)]
        public string UserEmailExplain { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0)]
        public string UserNameExplain { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0)]
        public string UserPhoneExplain { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0)]
        public string UserTextExplain { get; set; }
        [Required]
        public bool IsCountLimit { get; set; }
        public int CountLimit { get; set; }
        public string ReturnSuccess { get; set; }
        public string ReturnFail { get; set; }
    }
}

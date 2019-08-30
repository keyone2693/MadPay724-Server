using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MadPay724.Data.Models.UserModel;
using Microsoft.AspNetCore.Identity;

namespace MadPay724.Data.Models
{
    public class User : IdentityUser
    {

        [Required]
        [StringLength(100, MinimumLength = 0)]
        public string Name { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Address { get; set; }
        [Required]
        public bool Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastActive { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string City { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool Status { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<BankCard> BankCards { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Token> Tokens { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}

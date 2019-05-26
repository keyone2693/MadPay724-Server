using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models
{
    public class User : BaseEntity<string>
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string City { get; set; }
        [Required]
        public bool IsAcive { get; set; }
        [Required]
        public bool Status { get; set; }

        public ICollection<Photo> Phohots { get; set; }
        public ICollection<BankCard> BankCards { get; set; }
    }
}

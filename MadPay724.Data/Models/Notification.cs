using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models
{
    public class Notification : BaseEntity<string>
    {

        public Notification()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public bool EnterEmail { get; set; }
        [Required]
        public bool EnterSms { get; set; }
        [Required]
        public bool EnterTelegram { get; set; }

        [Required]
        public bool ExitEmail { get; set; }
        [Required]
        public bool ExitSms { get; set; }
        [Required]
        public bool ExitTelegram { get; set; }


        [Required]
        public bool TicketEmail { get; set; }
        [Required]
        public bool TicketSms { get; set; }
        [Required]
        public bool TicketTelegram { get; set; }

        [Required]
        public bool LoginEmail { get; set; }
        [Required]
        public bool LoginSms { get; set; }
        [Required]
        public bool LoginTelegram { get; set; }


        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}

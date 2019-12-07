using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models.MainDB.Blog
{
   public class Blog : BaseEntity<string>
    {
        public Blog()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Tags { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string PicAddress { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string SummerText { get; set; }
        [Required]
        public bool IsSelected { get; set; }
        [Required]
        public int ViewCount { get; set; }


        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string BlogGroupId { get; set; }
        [ForeignKey("BlogGroupId")]
        public BlogGroup BlogGroup { get; set; }


    }
}

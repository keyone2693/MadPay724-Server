using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models.Blog
{
  public  class BlogGroup : BaseEntity<string>
    {
        public BlogGroup()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public short Parent { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 0)]
        public string Name { get; set; }

        public ICollection<Blog> Blogs { get; set; }

    }
}

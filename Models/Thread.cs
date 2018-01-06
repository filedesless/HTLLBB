using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HTLLBB.Models
{
    public class Thread
    {
        public Thread() => Posts = new List<Post>();

        [Required]
        public int ID { get; set; }
        [Required]
        public String Title { get; set; }

		public ICollection<Post> Posts { get; set; }

        [Required]
        public int ForumId { get; set; }
        [Required]
        public Forum Forum { get; set; }
    }
}

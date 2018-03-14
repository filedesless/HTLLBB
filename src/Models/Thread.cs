using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class Thread
    {
        public Thread() => Posts = new List<Post>();

        [Required]
        public int ID { get; set; }
        [Required]
        public String Title { get; set; }

		[Required]
		public ApplicationUser Author { get; set; }
		
        [Required]
        public String Content { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

		public ICollection<Post> Posts { get; set; }

        [Required]
        public int ForumId { get; set; }
        [Required]
        public Forum Forum { get; set; }
    }
}

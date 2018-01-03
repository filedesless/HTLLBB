using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Thread
    {
        public Thread() => Posts = new List<Post>();

        public int ID { get; set; }
        public String Title { get; set; }

		public ICollection<Post> Posts { get; set; }

        public int ForumId { get; set; }
        public Forum Forum { get; set; }
    }
}

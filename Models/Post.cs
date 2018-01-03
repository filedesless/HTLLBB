using System;

namespace HTLLBB.Models
{
    public class Post
    {
        public int ID { get; set; }
        public ApplicationUser Author { get; set; }
        public String Content { get; set; }
        public DateTime CreationTime { get; set; }

        public int ThreadId { get; set; }
        public Thread Thread { get; set; }
    }
}

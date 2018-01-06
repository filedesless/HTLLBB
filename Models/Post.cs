using System;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class Post
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public ApplicationUser Author { get; set; }
        [Required]
        public String Content { get; set; }
        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        public int ThreadId { get; set; }
        [Required]
        public Thread Thread { get; set; }
    }
}

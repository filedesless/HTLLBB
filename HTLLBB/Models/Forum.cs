using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class Forum
    {
        public Forum() => Threads = new List<Thread>();

        [Required]
        public int ID { get; set; }
        [Required]
        public String Name { get; set; }

        public ICollection<Thread> Threads { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
    }
}

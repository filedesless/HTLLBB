using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Forum
    {
        public Forum() => Threads = new List<Thread>();

        public int ID { get; set; }
        public String Name { get; set; }

        public ICollection<Thread> Threads { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

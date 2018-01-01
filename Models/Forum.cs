using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Forum
    {
        public int ID { get; set; }
        public List<Thread> Threads { get; set; }
    }
}

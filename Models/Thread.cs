using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Thread
    {
        public int ID { get; set; }
        public List<Post> Posts { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Category
    {
        public int ID { get; set; }
        public List<Forum> Forums { get; set; }
    }
}

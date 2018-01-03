using System;
using System.Collections.Generic;

namespace HTLLBB.Models
{
    public class Category
    {
        public Category() => Forums = new List<Forum>();

        public int ID { get; set; }
        public String Name { get; set; }

        public ICollection<Forum> Forums { get; set; }
    }
}

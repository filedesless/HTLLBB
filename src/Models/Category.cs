using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class Category
    {
        public Category() => Forums = new List<Forum>();

        [Required]
        public int ID { get; set; }
        [Required]
        public String Name { get; set; }

        public ICollection<Forum> Forums { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ForumViewModels
{
    public class IndexViewModel
    {
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public Forum Forum { get; set; }
        [Required]
        public String UserId { get; set; }
    }
}

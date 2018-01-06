using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ForumViewModels
{
    public class IndexViewModel
    {
        public bool IsAdmin { get; set; }
        public Forum Forum { get; set; }
    }
}

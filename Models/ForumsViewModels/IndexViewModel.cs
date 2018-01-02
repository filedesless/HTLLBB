using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ForumsViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public bool IsAdmin { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ForumViewModels
{
    public class CreateViewModel
    {
        [Required]
        public int CatID { get; set; }
        [Required]
        public String Name { get; set; }
    }
}

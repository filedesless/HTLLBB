using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ThreadViewModels
{
    public class CreateViewModel
    {
        [Required]
        public int ForumID { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Content { get; set; }
    }
}

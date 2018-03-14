using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ThreadViewModels
{
    public class EditViewModel
    {
        [Required]
        [RegularExpression("[^/]+", ErrorMessage = "The field Title must not contain slashes")]
        public String Title { get; set; }

        [Required]
        public String Content { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.PostViewModels
{
    public class CreateViewModel
    {
        [Required]
        public int ThreadID { get; set; }
        [Required]
        public String Content { get; set; }
    }
}

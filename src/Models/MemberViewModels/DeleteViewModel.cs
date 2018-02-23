using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models.MemberViewModels
{
    public class DeleteViewModel
    {
        [Required]
        public String Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public String UserName { get; set; }
    }
}

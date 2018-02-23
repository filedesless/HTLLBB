using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models.MemberViewModels
{
    public class EditViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public String UserName { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public String Password { get; set; }
    }
}

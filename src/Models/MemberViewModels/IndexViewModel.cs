using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models.MemberViewModels
{
    public class Member
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Role { get; set; }
    }

    public class IndexViewModel
    {
        [Required]
        public bool IsAdmin { get; set; }

        // User:roles pair
        [Required]
        public IEnumerable<Member> Members { get; set; }
    }
}

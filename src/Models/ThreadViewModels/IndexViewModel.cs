using System;
namespace HTLLBB.Models.ThreadViewModels
{
    public class IndexViewModel
    {
        public bool IsAdmin { get; set; }
        public Thread Thread { get; set; }
        public String UserId { get; set; }
    }
}

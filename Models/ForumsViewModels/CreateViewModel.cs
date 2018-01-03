using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HTLLBB.Models.ForumsViewModels
{
    public class CreateViewModel
    {
        public int CatID { get; set; }
        public String Name { get; set; }
    }
}

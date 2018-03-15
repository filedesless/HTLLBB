using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class ChatboxChannel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public String Topic { get; set; }

        [Required]
        public List<ChatboxMessageBlock> Blocks { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class ChatboxMessage
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int BlockId { get; set; }
        [Required]
        public ChatboxMessageBlock Block { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
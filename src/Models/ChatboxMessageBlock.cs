using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class ChatboxMessageBlock
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int ChannelId { get; set; }
        [Required]
        public ChatboxChannel Channel { get; set; }

        [Required]
        public int AuthorId { get; set; }
        [Required]
        public ApplicationUser Author { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public List<ChatboxMessage> Messages { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace HTLLBB.Models
{
    public class ChatboxMessage
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
        public string Content { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
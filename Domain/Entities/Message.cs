using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }        
        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public MessageType Type { get; set; }
        public string? ContentType { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? EditedAt { get; set; }


        #region Relationships
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public Chat Chat { get; set; }
        public User Sender { get; set; }
        #endregion
    }
}

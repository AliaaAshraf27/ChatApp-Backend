using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class MessageDto
    {
        public Guid ChatId { get; set; }
        public string? Content { get; set; }
        public MessageType Type { get; set; }
        public string? MediaUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? ContentType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public SenderDto Sender { get; set; }
    }
}

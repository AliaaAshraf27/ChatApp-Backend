using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats.Dtos
{
    public class LastMessageDto
    {
        public string? Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}

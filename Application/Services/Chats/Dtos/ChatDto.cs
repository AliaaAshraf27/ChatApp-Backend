using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats.Dtos
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public ChatType Type { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public LastMessageDto? LastMessage { get; set; }
     
    }
}

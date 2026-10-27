using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats.Dtos
{
    public class UpdateGroupChatDto
    {
        public Guid ChatId { get; set; }
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}

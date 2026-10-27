using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ChatMembers.Dtos
{
    public class PinChatDto
    {
        public Guid ChatId { get; set; }
        public bool IsPinned { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats.Dtos
{
    public class MemberDto
    { 
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}

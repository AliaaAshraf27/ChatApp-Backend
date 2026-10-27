using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats.Dtos
{
    public class CreateGroupChatDto
    {
        public string GroupName { get; set; }
        public List<Guid> MemberIds { get; set; }
    }
}

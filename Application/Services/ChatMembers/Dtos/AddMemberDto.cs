using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ChatMembers.Dtos
{
    public class AddMemberDto
    {
        public List<Guid> UserId { get; set; }
        public Guid ChatId { get; set; }
    }
}

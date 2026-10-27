using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ChatMembers.Dtos
{
    public class MemberActionDto
    {
        public Guid ChatId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}

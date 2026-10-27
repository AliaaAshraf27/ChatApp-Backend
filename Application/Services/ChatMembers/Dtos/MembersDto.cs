using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ChatMembers.Dtos
{
    public class MembersDto
    {
        public string? UserName { get; set; } 
        public bool IsAdmin { get; set; }
        public string? Image { get; set; }
    }
}

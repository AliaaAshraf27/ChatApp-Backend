using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserManagement.Dtos
{
    public class UserDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

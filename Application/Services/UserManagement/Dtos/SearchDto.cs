using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserManagement.Dtos
{
    public class SearchDto
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

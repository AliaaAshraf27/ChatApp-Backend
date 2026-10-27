using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserManagement.Dtos
{
    public class FilterDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

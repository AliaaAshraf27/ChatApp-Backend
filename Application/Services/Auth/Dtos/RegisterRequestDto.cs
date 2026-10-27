using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Auth.Dtos
{
    public class RegisterRequestDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}

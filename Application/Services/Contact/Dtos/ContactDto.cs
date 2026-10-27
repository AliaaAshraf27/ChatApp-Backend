using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Contact.Dtos
{
    public class ContactDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
    }
}

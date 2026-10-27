using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Auth.Dtos
{
    public class ProfileDto
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public string? Bio { get; set; }
    }
}

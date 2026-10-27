using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class SendMessageDto
    {
        public Guid ChatId { get; set; }
        public string? Content { get; set; }
        public IFormFile? Media { get; set; } // image or file
       
    }

}

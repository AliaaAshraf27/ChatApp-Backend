using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
        public class ResponseSearchDto
        {
            public string? Content { get; set; }
            public string? MediaUrl { get; set; }
            public string? ChatName { get; set; }
            public string? SenderName { get; set; }
            public DateTime? SentAt { get; set; }
        }
}

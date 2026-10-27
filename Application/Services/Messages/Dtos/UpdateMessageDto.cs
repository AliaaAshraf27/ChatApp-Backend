using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class UpdateMessageDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public string? Content { get; set; }
    }
}

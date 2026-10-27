using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class ForwardMessageDto
    {
        public Guid MessageId { get; set; }
        public Guid CurrentChatId { get; set; }
        public List<Guid> ChatIds { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class MessageActionDto
    {
      public Guid chatId { get; set; }
      public Guid messageId { get; set; }
    }
}

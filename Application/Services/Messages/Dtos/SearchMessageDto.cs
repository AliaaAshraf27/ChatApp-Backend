using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class SearchMessageDto
    {
        public Guid ChatId { get; set; }
        public string keyword{ get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages.Dtos
{
    public class MessageFilterDto
    {
        public Guid chatId { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; }

    }
}

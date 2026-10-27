using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status.Dtos
{
    public class UserStatusDto
    {
        public Guid StatusId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string? UserImage { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsViewed { get; set; }

    }
}

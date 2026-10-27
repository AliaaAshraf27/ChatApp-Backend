using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserBlock
    {
        public Guid Id { get; set; }          
        public Guid UserId { get; set; } 
        public Guid BlockedUserId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public User BlockedUser { get; set; }
    }
}

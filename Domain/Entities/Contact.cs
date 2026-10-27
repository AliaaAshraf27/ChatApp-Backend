using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ContactUserId { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Relationships
        public User User { get; set; } 
        public User ContactUser { get; set; }
        #endregion
    }
}

using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Status
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; }
        public string? Content { get; set; }       
        public string? MediaUrl { get; set; }   
        public StatusType Type { get; set; }
        public string? BackgroundColor { get; set; } 
        public string? FontStyle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } 
        public bool IsDeleted { get; set; } = false;

        #region Relationships
        public User User { get; set; }
        public ICollection<StatusView> Views { get; set; }
        #endregion
    }


}

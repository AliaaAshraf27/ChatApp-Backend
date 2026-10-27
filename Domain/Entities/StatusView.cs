using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StatusView
    {
        public Guid Id { get; set; } 
        public Guid StatusId { get; set; }
        public Guid ViewerId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        #region Relationships
        public Status Status { get; set; }
        public User Viewer { get; set; }
        #endregion
    }
}

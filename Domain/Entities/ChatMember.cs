using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMember // m to m
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMuted { get; set; } = false;
        public bool IsPinned { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public DateTime JoinedAt { get; set; }
        public DateTime? LastDeliveredAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public DateTime? RemovedAt { get; set; }

        #region Relationships
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
        #endregion
    }
}

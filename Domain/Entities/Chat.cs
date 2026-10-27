using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public ChatType Type { get; set; }
        public string? Name { get; set; }
        public byte[]? Image { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Relationships
        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Message> Messages { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public byte[]? Image { get; set; }
        public string? Bio { get; set; }
        public bool IsOnline { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastSeen { get; set; }

        #region Relationships
        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<UserBlock> BlockedByMe { get; set; }
        public ICollection<UserBlock> BlockedMe { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        #endregion
    }
}

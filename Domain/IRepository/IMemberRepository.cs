using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IMemberRepository
    {
        Task<List<ChatMember>> GetMembersAsync(Guid chatId);
        Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId);
        Task<ChatMember?> GetOtherMemberAsync(Guid chatId, Guid userId);
        Task<List<Guid>> GetExistingMemberIdsAsync(Guid chatId, List<Guid> userIds);
        Task<List<Guid>> GetMemberIdsAsync(Guid chatId);
        //Task<Guid?> GetOtherMemberIdAsync(Guid chatId, Guid userId);
        Task<ChatMember?> GetOtherAdminAsync(Guid chatId, Guid excludeUserId);
        Task<bool> IsMemberAsync(Guid chatId, Guid userId);
        Task<bool> IsAdminAsync(Guid chatId, Guid userId);
        Task UpdateAsync(ChatMember member);
        Task<bool> UpdateLastReadAtAsync(Guid chatId, Guid userId, DateTime at);
        Task<bool> UpdateLastDeliveredAtAsync(Guid chatId, Guid userId, DateTime at);
        Task AddMembersAsync(List<ChatMember> Members);
        Task<Dictionary<Guid, Guid>> GetPrivateChatOtherMemberIdsAsync(Guid userId);

    }
}

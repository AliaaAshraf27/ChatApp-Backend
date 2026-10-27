using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MemberRepository(ApplicationDbContext _context) : IMemberRepository
    {
        public async Task<List<ChatMember>> GetMembersAsync(Guid chatId)
          => await _context.ChatMembers
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ChatId == chatId && !x.IsDeleted)
            .ToListAsync();
        public async Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId)
           => await _context.ChatMembers
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.UserId == userId && !m.IsDeleted);
        public async Task<ChatMember?> GetOtherMemberAsync(Guid chatId, Guid userId)
           => await _context.ChatMembers
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.UserId != userId && !m.IsDeleted);
        public async Task<List<Guid>> GetExistingMemberIdsAsync(Guid chatId, List<Guid> userIds)
             => await _context.ChatMembers
            .Where(m => m.ChatId == chatId && 
                   userIds.Contains(m.UserId) && !m.IsDeleted)
            .Select(m => m.UserId)
            .ToListAsync();
        public async Task<List<Guid>> GetMemberIdsAsync(Guid chatId)
             => await _context.ChatMembers
               .Where(m => m.ChatId == chatId && !m.IsDeleted)
               .Select(m => m.UserId)
               .ToListAsync();
        public async Task<ChatMember?> GetOtherAdminAsync(Guid chatId, Guid excludeUserId)
           => await _context.ChatMembers
              .FirstOrDefaultAsync(m => m.ChatId == chatId &&
               m.UserId != excludeUserId && m.IsAdmin &&!m.IsDeleted);
        public async Task<Dictionary<Guid, Guid>> GetPrivateChatOtherMemberIdsAsync(Guid userId)
            => await _context.ChatMembers
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.UserId != userId &&
                    m.Chat.Type == ChatType.Private &&
                    m.Chat.ChatMembers.Any(x => x.UserId == userId && !x.IsDeleted))
                .ToDictionaryAsync(m => m.ChatId, m => m.UserId);
        public async Task<bool> IsMemberAsync(Guid chatId, Guid userId)
            => await _context.ChatMembers
            .AnyAsync(m => m.ChatId == chatId && m.UserId == userId && !m.IsDeleted);
        public async Task<bool> IsAdminAsync(Guid chatId, Guid userId)
          => await _context.ChatMembers
            .AnyAsync(m => m.ChatId == chatId && m.UserId == userId && m.IsAdmin && !m.IsDeleted);
        public async Task UpdateAsync(ChatMember member)
        {
            _context.ChatMembers.Update(member);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateLastReadAtAsync(Guid chatId, Guid userId, DateTime at)
        {
            var rows = await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId && cm.UserId == userId && !cm.IsDeleted
                          && (cm.LastReadAt == null || cm.LastReadAt < at)) 
                .ExecuteUpdateAsync(s => s
                    .SetProperty(cm => cm.LastReadAt, at)
                    .SetProperty(cm => cm.LastDeliveredAt,
                        cm => cm.LastDeliveredAt == null || cm.LastDeliveredAt < at ? at : cm.LastDeliveredAt));
            return rows > 0;
        }
        public async Task<bool> UpdateLastDeliveredAtAsync(Guid chatId, Guid userId, DateTime at)
        {
            var rows = await _context.ChatMembers
                .Where(cm => cm.ChatId == chatId && cm.UserId == userId && !cm.IsDeleted
                          && (cm.LastDeliveredAt == null || cm.LastDeliveredAt < at))
                .ExecuteUpdateAsync(s => s.SetProperty(cm => cm.LastDeliveredAt, at));
            return rows > 0;
        }
        public async Task AddMembersAsync(List<ChatMember> Members)
        {
            await _context.ChatMembers.AddRangeAsync(Members);
            await _context.SaveChangesAsync();
        }
    }
}

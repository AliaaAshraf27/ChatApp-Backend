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
    public class ChatRepository(ApplicationDbContext _context) : IChatRepository
    {
        public async Task<List<Chat>> GetUserChatsAsync(Guid userId)
              => await _context.Chats.Where(c => c.ChatMembers.Any(m => m.UserId == userId && !m.IsDeleted))
                     .Include(c => c.ChatMembers).ThenInclude(m => m.User)
                     .Include(c => c.Messages.OrderByDescending(x => x.SentAt).Take(1))
                     .ToListAsync();
        public async Task<Chat?> GetByIdAsync(Guid Id)
              => await _context.Chats
                     .Include(c => c.ChatMembers).ThenInclude(m => m.User)
                     .Include(c => c.Messages.OrderByDescending(x => x.SentAt).Take(1))
                     .FirstOrDefaultAsync(c => c.Id == Id);
        public async Task<Chat?> GetPrivateChatAsync(Guid currentId, Guid targetId)
            => await _context.Chats.AsNoTracking()
            .Where(c => c.Type == ChatType.Private &&
                c.ChatMembers.Any(m => m.UserId == currentId && !m.IsDeleted) &&
                c.ChatMembers.Any(m => m.UserId == targetId && !m.IsDeleted))
               .Include(c => c.ChatMembers).FirstOrDefaultAsync();
        public async Task<List<Chat>> GetArchivedChatsAsync(Guid userId)
              => await _context.Chats.Where(c => c.ChatMembers
                     .Any(m => m.UserId == userId && m.IsArchived && !m.IsDeleted))
                     .Include(c => c.ChatMembers).ThenInclude(m => m.User)
                     .Include(c => c.Messages.OrderByDescending(x => x.SentAt).Take(1))
                     .ToListAsync();
        public async Task<List<Chat>> GetPinnedChatsAsync(Guid userId)
            => await _context.Chats.Where(c => c.ChatMembers
                   .Any(m => m.UserId == userId && m.IsPinned && !m.IsDeleted))
                   .Include(c => c.ChatMembers).ThenInclude(m => m.User)
                   .Include(c => c.Messages.OrderByDescending(x => x.SentAt).Take(1))
                   .ToListAsync();
        public async Task AddAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Chat chat)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Guid>> GetChatIdsAsync(Guid userId)
        {
            return await _context.ChatMembers
                .Where(m => m.UserId == userId && !m.IsDeleted)
                .Select(m => m.ChatId)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId)
        {
            var chatIds = _context.ChatMembers
                .Where(m => m.UserId == userId && !m.IsDeleted)
                .Select(m => m.ChatId);

            return await _context.ChatMembers
                .Where(m =>
                    chatIds.Contains(m.ChatId) &&
                    m.UserId != userId &&
                    !m.IsDeleted)
                .Select(m => m.UserId)
                .Distinct()
                .ToListAsync();
        }
      
    }
}

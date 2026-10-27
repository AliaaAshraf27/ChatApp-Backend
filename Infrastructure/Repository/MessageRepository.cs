using Domain.Entities;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Infrastructure.Repository
{
    public class MessageRepository(ApplicationDbContext _context) : IMessageRepository
    {
        public async Task<List<Message>> GetChatMessagesAsync(Guid chatId, int pageNumber, int pageSize)
            => await _context.Messages
             .Where(m => m.ChatId == chatId && !m.IsDeleted)
             .Include(m => m.Sender)
             .OrderByDescending(m => m.SentAt).Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();
        public async Task<Message?> GetByIdAsync(Guid Id) =>
            await _context.Messages
            .Include(m => m.Chat)
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == Id);
        public async Task<List<Message>> SearchAsync(string keyword, Guid chatId)
           => await _context.Messages
            .AsNoTracking()
            .Where(m =>
                m.ChatId == chatId && !m.IsDeleted &&
                m.Content != null && m.Content.Contains(keyword))
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Message>> GetUnreadMessagesAsync(Guid userId)
           => await _context.Messages
              .Include(m => m.Sender)
              .Include(m => m.Chat)
                  .ThenInclude(c => c.ChatMembers)
              .Where(m =>!m.IsDeleted && m.SenderId != userId && m.Chat.ChatMembers.Any(cm =>
                      cm.UserId == userId && !cm.IsDeleted 
              && (cm.LastReadAt == null ||m.SentAt > cm.LastReadAt)))
              .OrderBy(m => m.SentAt)
              .ToListAsync();
       

    }
}

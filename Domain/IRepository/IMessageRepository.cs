using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetChatMessagesAsync(Guid chatId,int pageNumber,int pageSize);
        Task<Message?> GetByIdAsync(Guid Id);
        Task<List<Message>> SearchAsync(string keyword, Guid chatId);
        Task AddAsync(Message message);
        Task UpdateAsync(Message message);
        Task DeleteAsync(Message message);
        Task<List<Message>> GetUnreadMessagesAsync(Guid userId);
      
    }
}

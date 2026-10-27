using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IChatRepository
    {
        Task<List<Chat>> GetUserChatsAsync(Guid userId);
        Task<Chat?> GetByIdAsync(Guid Id);
        Task<Chat?> GetPrivateChatAsync(Guid currentId, Guid targetId);
        //Task<List<Chat>> GetAllPrivateChatsAsync(Guid userId);
        Task<List<Chat>> GetArchivedChatsAsync(Guid userId);
        Task<List<Chat>> GetPinnedChatsAsync(Guid userId);
        Task AddAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task DeleteAsync(Chat chat);
        Task<List<Guid>> GetChatIdsAsync(Guid userId);
        Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId);
    }
}

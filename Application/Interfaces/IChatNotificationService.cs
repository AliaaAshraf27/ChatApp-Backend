using Application.Services.Chats.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatNotificationService
    {
        Task SendNewChatCreatedAsync(Guid chatId, IReadOnlyDictionary<Guid, ChatDto> chatPerUser);
        Task SendGroupUpdatedAsync(Guid chatId, ChatDto chatDto);
        Task SendChatDeletedAsync(Guid chatId, IEnumerable<Guid> memberIds);
    }
}

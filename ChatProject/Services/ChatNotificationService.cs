using Api.SignalR;
using Application.Interfaces;
using Application.Services.Chats.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services
{
    public class ChatNotificationService(IHubContext<ChatHub> hub, IUserConnectionTracker tracker)
    : IChatNotificationService
    {
        public async Task SendNewChatCreatedAsync(Guid chatId, IReadOnlyDictionary<Guid, ChatDto> chatPerUser)
        {
            foreach (var (userId, chatDto) in chatPerUser)
            {
                foreach (var connectionId in tracker.GetConnections(userId))
                    await hub.Groups.AddToGroupAsync(connectionId, chatId.ToString());

                await hub.Clients.User(userId.ToString()).SendAsync("ChatCreated", chatDto);
            }
        }
        public Task SendGroupUpdatedAsync(Guid chatId, ChatDto chatDto)
            => hub.Clients.Group(chatId.ToString()).SendAsync("GroupUpdated", chatDto);
        public async Task SendChatDeletedAsync(Guid chatId, IEnumerable<Guid> memberIds)
        {
            await hub.Clients.Group(chatId.ToString()).SendAsync("ChatDeleted", chatId);

            foreach (var userId in memberIds.Distinct())
                foreach (var connectionId in tracker.GetConnections(userId))
                    await hub.Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());
        }
      
        }
    }
    

using Api.SignalR;
using Application.Interfaces;
using Application.Services.Chats.Dtos;
using Application.Services.UserManagement.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services
{
    public class MemberNotificationService(IHubContext<ChatHub> _hub, IUserConnectionTracker _userConnection) : IMemberNotificationService
    {
        public async Task SendMemberAddedAsync(Guid chatId, ChatDto model, Guid userId)
        {
            await _hub.Clients.Group(chatId.ToString())
              .SendAsync("MemberAdded", new { chatId, userId });
            foreach (var connectionId in _userConnection.GetConnections(userId))
            {
                await _hub.Groups.AddToGroupAsync(
                    connectionId,
                    chatId.ToString());
            }
            await _hub.Clients.User(userId.ToString())
                .SendAsync("GroupMemberAdded", model);
        }
        public async Task SendMemberRemovedAsync(Guid chatId, Guid userId)
        {
            await _hub.Clients.User(userId.ToString())
                .SendAsync("RemovedFromGroup", chatId);

            foreach (var connectionId in _userConnection.GetConnections(userId))
                await _hub.Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());

            await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MemberRemoved", new { userId, chatId });
        }
        public async Task SendMemberLeftAsync(Guid chatId, Guid userId)
        {
            foreach (var connectionId in _userConnection.GetConnections(userId))
                await _hub.Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());

            await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MemberLeft", new { userId, chatId });
        }
        public async Task SendPromotedAsync(Guid chatId, Guid userId)
            => await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MemberPromoted", new { chatId, userId });
        public async Task SendDemotedAsync(Guid chatId, Guid userId)
            => await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MemberDemoted", new { chatId, userId });

    }
}

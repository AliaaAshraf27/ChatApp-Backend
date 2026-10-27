using Api.SignalR;
using Application.Interfaces;
using Application.Services.Contact.ContactName;
using Application.Services.Messages.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Microsoft.AspNetCore.SignalR;
namespace Api.Services

{
    public class MessageNotificationService(IHubContext<ChatHub> _hub,IUserConnectionTracker _userConnection,
    IContactNameService _nameService,IMemberRepository _memberRepo) : IMessageNotificationService
    {
        public async Task SendMessageAsync(MessageDto model)
        {
            var members = await _memberRepo.GetMembersAsync(model.ChatId);
            foreach (var member in members)
            {
                var recipientMessage = System.Text.Json.JsonSerializer
                    .Deserialize<MessageDto>(
                        System.Text.Json.JsonSerializer.Serialize(model))!;
                if (recipientMessage.Sender != null)
                    await _nameService.ApplySenderNamesAsync(new[] { recipientMessage.Sender },member.UserId);

                var connections = _userConnection.GetConnections(member.UserId);
                if (connections.Count > 0)
                    await _hub.Clients.Clients(connections).SendAsync("ReceiveMessage", recipientMessage);
            }
        }
        public async Task SendMessageUpdatedAsync(MessageDto model)
        {
            var members = await _memberRepo.GetMembersAsync(model.ChatId);
            foreach (var member in members)
            {
                var recipientMessage = System.Text.Json.JsonSerializer
                    .Deserialize<MessageDto>(System.Text.Json.JsonSerializer.Serialize(model))!;

                if (recipientMessage.Sender != null)
                    await _nameService.ApplySenderNamesAsync(new[] { recipientMessage.Sender },member.UserId);
                
                var connections = _userConnection.GetConnections(member.UserId);
                if (connections.Count > 0)
                    await _hub.Clients.Clients(connections)
                        .SendAsync("MessageUpdated", recipientMessage);
            }
        }
        public async Task SendMessageDeletedAsync(Guid chatId, Guid messageId)
            => await _hub.Clients.Group(chatId.ToString())
                .SendAsync("MessageDeleted", new { chatId, messageId });
        public Task SendMessagesDeliveredAsync(Guid chatId, Guid lastMessageId, Guid deliveredToUserId)
           => _hub.Clients.Group(chatId.ToString())
              .SendAsync("MessagesDelivered",
                new { chatId, lastMessageId, deliveredToUserId, deliveredAt = DateTime.UtcNow });
        public async Task SendMessagesReadAsync(Guid chatId,Guid lastMessageId,Guid readByUserId)
        {
            var readerConnections = _userConnection.GetConnections(readByUserId);
            await _hub.Clients
                .GroupExcept(chatId.ToString(), readerConnections)
                .SendAsync("MessagesRead", new
                {
                    chatId,
                    lastMessageId,
                    readByUserId,
                    readAt = DateTime.UtcNow
                });
        }

    }
}

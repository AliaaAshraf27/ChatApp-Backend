using Application.Services.ChatMembers;
using Application.Services.Chats;
using Application.Services.Messages;
using Application.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Api.SignalR
{
    [Authorize]
    public class ChatHub(
        IChatService chatService,
        IUserService userService,
        IMemberService memberService,
        IMessageService messageService,
        IUserConnectionTracker connections,
        ILogger<ChatHub> logger) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (!TryGetUserId(out var userId))
            {
                Context.Abort();
                return;
            }
            try
            {
                var isFirstConnection = connections.Add(userId, Context.ConnectionId);
                var ids = connections.GetConnections(userId);
                var chatIds = await chatService.GetChatIdsAsync(userId);
                foreach (var chatId in chatIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId,chatId.ToString());
                }
                if (isFirstConnection)
                {
                    await userService.SetOnlineAsync(userId);
                    var relatedUserIds = await chatService.GetRelatedUserIdsAsync(userId);

                    await Clients.Users(relatedUserIds.Select(id => id.ToString()))
                        .SendAsync("UserOnline", new { userId });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"OnConnectedAsync failed for {UserId}",userId);
                connections.Remove(userId, Context.ConnectionId);
                Context.Abort();
                return;
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (TryGetUserId(out var userId))
            {
                var isLastConnection =connections.Remove(userId, Context.ConnectionId);
                if (isLastConnection)
                {
                    try
                    {
                        var lastSeen = await userService.SetOfflineAsync(userId);
                        var relatedUserIds =await chatService.GetRelatedUserIdsAsync(userId);

                        await Clients.Users(relatedUserIds.Select(id => id.ToString()))
                            .SendAsync("UserOffline", new { userId, lastSeen });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,"OnDisconnectedAsync failed for {UserId}",userId);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task Typing(Guid chatId)
        {
            if (!TryGetUserId(out var userId))
                return;

            if (!await memberService.IsMemberAsync(chatId, userId))
                return;

            await Clients.OthersInGroup(chatId.ToString())
                .SendAsync("UserTyping", new { chatId, userId });
        }
        public async Task StopTyping(Guid chatId)
        {
            if (!TryGetUserId(out var userId))
                return;

            if (!await memberService.IsMemberAsync(chatId, userId))
                return;

            await Clients.OthersInGroup(chatId.ToString())
                .SendAsync("UserStoppedTyping", new { chatId, userId });
        }
        public async Task MarkAsDelivered(Guid chatId, Guid lastMessageId)
        {
            if (!TryGetUserId(out var userId))
                return;

            if (!await memberService.IsMemberAsync(chatId, userId))
                return;

            await messageService.MarkAsDeliveredAsync(userId,chatId,lastMessageId);
        }

        private bool TryGetUserId(out Guid userId)
        {
            return Guid.TryParse(
                Context.UserIdentifier,
                out userId);
        }
    }
}

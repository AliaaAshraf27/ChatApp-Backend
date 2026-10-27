using Application.Helpers;
using Application.Interfaces;
using Application.Response;
using Application.Services.Contact.ContactName;
using Application.Services.File;
using Application.Services.Messages.Dtos;
using Application.Services.UserManagement.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Application.Services.Messages
{
    public class MessageService(
        IMessageRepository _messageRepo,
        IUserRepository _userRepo ,
        IChatRepository _chatRepo,
        IMemberRepository _memberRepo,
        IMapper _mapper ,
        IFileStorageService _fileService,
        IContactNameService _nameService,
        IBlockedUserRepository _blockedUserRepo ,
        IMessageNotificationService _notification,
        ILogger<MessageService> logger) :IMessageService
    {
        public async Task<ApiResponse<PagedResult<MessageDto>>> GetChatMessagesAsync
            (MessageFilterDto dto, Guid userId)
        {
            if (!await _memberRepo.IsMemberAsync(dto.chatId, userId))
                return ApiResponse<PagedResult<MessageDto>>.FailureResponse("Access denied");

            var messages = await _messageRepo.GetChatMessagesAsync(dto.chatId, dto.pageNumber, dto.pageSize);
            var items = _mapper.Map<List<MessageDto>>(messages);
            await _nameService.ApplySenderNamesAsync
                (items.Where(m => m.Sender != null).Select(m => m.Sender),userId);

            var result = new PagedResult<MessageDto>
            {
                Items = items,
                PageNumber = dto.pageNumber ,
                PageSize = dto.pageSize
            };
           
            return ApiResponse<PagedResult<MessageDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<MessageDto>> GetByIdAsync(Guid messageId, Guid userId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null || message.IsDeleted)
                return ApiResponse<MessageDto>.FailureResponse("Message not found");

            if (!await _memberRepo.IsMemberAsync(message.ChatId, userId))
                return ApiResponse<MessageDto>.FailureResponse("Access denied");

            var result = _mapper.Map<MessageDto>(message);
            if (result.Sender != null)
            {
                await _nameService.ApplySenderNamesAsync(new[] { result.Sender },userId);
            }

            return ApiResponse<MessageDto>.SuccessResponse(result);

        }
        public async Task<ApiResponse<MessageDto>> SendMessageAsync(Guid userId, SendMessageDto model)
        {
            var chat = await _chatRepo.GetByIdAsync(model.ChatId);
            if (chat == null)
                return ApiResponse<MessageDto>.FailureResponse("Chat not found");
            if (!await _memberRepo.IsMemberAsync(model.ChatId, userId))
                return ApiResponse<MessageDto>.FailureResponse("Access denied");
            if (string.IsNullOrWhiteSpace(model.Content) && model.Media == null)
                return ApiResponse<MessageDto>.FailureResponse("Message must have content or media");
            if (chat.Type == ChatType.Private)
            {
                var otherUser = await _memberRepo.GetOtherMemberAsync(model.ChatId, userId);
                if (otherUser == null)
                    return ApiResponse<MessageDto>.FailureResponse("Chat member not found");

                if (await _blockedUserRepo.IsBlockedAsync(userId, otherUser.UserId))
                    return ApiResponse<MessageDto>.FailureResponse("Cannot send messages to this user");
            }

            var message = _mapper.Map<Message>(model);
            message.SenderId = userId;
            message.SentAt = DateTime.UtcNow;
            message.Type = DetermineMessageType(model.Media);

            if (model.Media != null)
            {
                const long maxFileSize = 50 * 1024 * 1024;
                if (model.Media.Length > maxFileSize)
                    return ApiResponse<MessageDto>.FailureResponse("File size exceeds the allowed limit");

                var allowedTypes = new[]
                {
                    "image/",
                    "application/pdf",
                    "application/msword",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/vnd.ms-excel",
                     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                if (!allowedTypes.Any(type => model.Media.ContentType.StartsWith(type)))
                    return ApiResponse<MessageDto>.FailureResponse("Unsupported file type");
                try
                {
                    var uploadResult =await _fileService.UploadAsync(model.Media,"chat-media");
                    message.MediaUrl = uploadResult.Url;
                    message.FileName = uploadResult.FileName;
                    message.FileSizeBytes = uploadResult.FileSizeBytes;
                    message.ContentType = uploadResult.ContentType;
                }
                catch (Exception)
                {
                    return ApiResponse<MessageDto>.FailureResponse("Failed to upload media");
                }
            }

            await _messageRepo.AddAsync(message);
            var result = _mapper.Map<MessageDto>(message);
            if (result.Sender != null)
            {
                await _nameService.ApplySenderNamesAsync(new[] { result.Sender },userId);
            }

            await NotifyAsync(()
                => _notification.SendMessageAsync(result));
            return ApiResponse<MessageDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<MessageDto>> UpdateMessageAsync(Guid userId, UpdateMessageDto model)
        {
            var message = await _messageRepo.GetByIdAsync(model.MessageId);
            if (message == null)
                return ApiResponse<MessageDto>.FailureResponse("Message not found");
            if (message.SenderId != userId)
                return ApiResponse<MessageDto>.FailureResponse("Access denied");
            if (message.IsDeleted)
                return ApiResponse<MessageDto>.FailureResponse("Cannot edit deleted message");
            if (message.Type != MessageType.Text)
                return ApiResponse<MessageDto>.FailureResponse("Only text messages can be edited");
            var chat = await _chatRepo.GetByIdAsync(model.ChatId);
            if (chat?.Type == ChatType.Private)
            {
                var otherUser = await _memberRepo.GetOtherMemberAsync(model.ChatId, userId);
                if (otherUser == null)
                    return ApiResponse<MessageDto>.FailureResponse("Chat member not found");

                if (await _blockedUserRepo.IsBlockedAsync(userId, otherUser.UserId))
                    return ApiResponse<MessageDto>.FailureResponse("Cannot forward message to a blocked user");
            }
            message.Content = model.Content;
            message.EditedAt = DateTime.UtcNow;
            message.IsEdited = true;
            await _messageRepo.UpdateAsync(message);
            var result = _mapper.Map<MessageDto>(message);
            if (result.Sender != null)
            {
                await _nameService.ApplySenderNamesAsync(new[] { result.Sender }, userId);
            }

            await NotifyAsync(() => _notification.SendMessageUpdatedAsync(result));
            return ApiResponse<MessageDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<string>> DeleteMessageAsync(Guid userId , MessageActionDto model)
        {
            var message = await _messageRepo.GetByIdAsync(model.messageId);
            if (message == null || message.ChatId != model.chatId)
                return ApiResponse<string>.FailureResponse("Message not found");

            var isMember = await _memberRepo.IsMemberAsync(message.ChatId, userId);
            if (!isMember)
                return ApiResponse<string>.FailureResponse("Access denied");

            if (message.IsDeleted)
                return ApiResponse<string>.FailureResponse("Message already deleted");

            var isOwner = message.SenderId == userId;
            if (!isOwner)
            {
                var isGroupAdmin =await _memberRepo.IsAdminAsync(message.ChatId, userId);

                if (message.Chat.Type != ChatType.Group || !isGroupAdmin)
                    return ApiResponse<string>.FailureResponse("You can only delete your own messages");
            }
            message.IsDeleted = true;
            message.Content = null;
            await _messageRepo.UpdateAsync(message);
            await NotifyAsync(() => _notification.SendMessageDeletedAsync(message.ChatId, message.Id));
            return ApiResponse<string>.SuccessResponse(null, "Message deleted");
        }
        public async Task<ApiResponse<string>> MarkAsReadAsync(MessageActionDto model, Guid userId)
        {
            var message = await _messageRepo.GetByIdAsync(model.messageId);
            if (message == null || message.ChatId != model.chatId)
                return ApiResponse<string>.FailureResponse("Message not found");

            var isMember = await _memberRepo.IsMemberAsync(model.chatId, userId);
            if (!isMember)
                return ApiResponse<string>.FailureResponse("Access denied");

            var changed = await _memberRepo.UpdateLastReadAtAsync(model.chatId, userId, message.SentAt);
            if (changed)
            {
                await NotifyAsync(() =>
                      _notification.SendMessagesReadAsync(model.chatId, message.Id, userId));
            }
            return ApiResponse<string>.SuccessResponse(null, "Marked as read");
        }
        public async Task<ApiResponse<string>> MarkAsDeliveredAsync(Guid userId, Guid chatId, Guid lastMessageId)
        {
            var message = await _messageRepo.GetByIdAsync(lastMessageId);
            if (message == null || message.ChatId != chatId)
                return ApiResponse<string>.FailureResponse("Message not found");

            if (!await _memberRepo.IsMemberAsync(chatId, userId))
                return ApiResponse<string>.FailureResponse("Access denied");

            var changed = await _memberRepo.UpdateLastDeliveredAtAsync(chatId, userId, message.SentAt);
            if (changed)
                await NotifyAsync(() => _notification.SendMessagesDeliveredAsync(chatId, message.Id, userId));

            return ApiResponse<string>.SuccessResponse(null, "Marked as delivered");
        }
        public async Task<ApiResponse<List<ResponseSearchDto>>> SearchAsync(SearchMessageDto model)
        {
            if (string.IsNullOrWhiteSpace(model.keyword))
                return ApiResponse<List<ResponseSearchDto>>.FailureResponse("Keyword is required");

            var messages = await _messageRepo.SearchAsync(model.keyword, model.ChatId);
            if (!messages.Any())
                return ApiResponse<List<ResponseSearchDto>>.FailureResponse("No messages found");

            return ApiResponse<List<ResponseSearchDto>>.SuccessResponse(_mapper.Map<List<ResponseSearchDto>>(messages));
        }
        public async Task<ApiResponse<bool>> ForwardMessageAsync(Guid userId,ForwardMessageDto model)
        {
            if (model.ChatIds == null || !model.ChatIds.Any())
                return ApiResponse<bool>.FailureResponse("At least one destination chat is required");

            if (!await _memberRepo.IsMemberAsync(model.CurrentChatId, userId))
                return ApiResponse<bool>.FailureResponse("You are not a member of the source chat");
           
            var message = await _messageRepo.GetByIdAsync(model.MessageId);
            if (message == null || message.IsDeleted || message.ChatId != model.CurrentChatId)
                return ApiResponse<bool>.FailureResponse("Message not found in the source chat");
            
            var targetChatIds = model.ChatIds.Distinct().ToList();
            if (targetChatIds.Contains(model.CurrentChatId))
                return ApiResponse<bool>.FailureResponse("Cannot forward a message to the same chat");

            // Validate all destinations before creating any messages.
            foreach (var chatId in targetChatIds)
            {
                var chat = await _chatRepo.GetByIdAsync(chatId);
                if (!await _memberRepo.IsMemberAsync(chatId, userId))
                    return ApiResponse<bool>.FailureResponse(
                        "You are not a member of a destination chat");
                if (chat?.Type == ChatType.Private)
                {
                    var otherUser = await _memberRepo.GetOtherMemberAsync(chatId, userId);
                    if (otherUser == null)
                        return ApiResponse<bool>.FailureResponse("Chat member not found");

                    if (await _blockedUserRepo.IsBlockedAsync(userId, otherUser.UserId))
                        return ApiResponse<bool>.FailureResponse("Cannot forward message to a blocked user");
                }
            }
            foreach (var chatId in targetChatIds)
            {
                var forwardedMessage = _mapper.Map<Message>(message);
                forwardedMessage.Id = Guid.NewGuid();
                forwardedMessage.ChatId = chatId;
                forwardedMessage.SenderId = userId;
                forwardedMessage.SentAt = DateTime.UtcNow;
                forwardedMessage.IsDeleted = false;
                forwardedMessage.IsEdited = false;
                forwardedMessage.EditedAt = null;

                await _messageRepo.AddAsync(forwardedMessage);
                var dto = _mapper.Map<MessageDto>(forwardedMessage);
                await NotifyAsync(() => _notification.SendMessageAsync(dto));
            }
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<List<MessageDto>>> GetUnreadMessagesAsync(Guid userId)
        {
            var messages = await _messageRepo.GetUnreadMessagesAsync(userId);
            if (!messages.Any())
                return ApiResponse<List<MessageDto>>.FailureResponse("No unread messages found");

            var result = _mapper.Map<List<MessageDto>>(messages);
            await _nameService.ApplySenderNamesAsync(
                result.Where(m => m.Sender != null).Select(m => m.Sender),userId);

            return ApiResponse<List<MessageDto>>.SuccessResponse(result);
        }

        #region Private Methods
        private static MessageType DetermineMessageType(IFormFile? media)
        {
            if (media == null)
                return MessageType.Text;

            if (media.ContentType.StartsWith("image/"))
                return MessageType.Image;

            return MessageType.File;
        }
        private async Task NotifyAsync(Func<Task> send)
        {
            try { await send(); }
            catch (Exception ex) { logger.LogError(ex, "SignalR notification failed"); }
        }
        #endregion

    }
}

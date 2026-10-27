using Application.Interfaces;
using Application.Response;
using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats.Dtos;
using Application.Services.Contact;
using Application.Services.Contact.ContactName;
using Application.Services.Messages;
using AutoMapper;
using AutoMapper.Execution;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services.Chats
{
    public class ChatService(
        IChatRepository _chatRepo, 
        IUserRepository _userRepo, 
        IMemberRepository _memberRepo, 
        IMapper _mapper,
        IChatNotificationService _notification,
        IContactService _contactService,
        IContactNameService _nameService,
        IBlockedUserRepository _blockedUserRepo,
        ILogger<ChatService> logger) :IChatService
    {
        public async Task<ApiResponse<List<ChatDto>>> GetUserChatsAsync(Guid userId)
        {
            var chats = await _chatRepo.GetUserChatsAsync(userId);
            var result = _mapper.Map<List<ChatDto>>(chats);

            var otherMemberIds =await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);
            var displayNames =await _nameService.GetDisplayNamesAsync(userId, otherMemberIds.Values);
            foreach (var chatDto in result)
            {
                if (otherMemberIds.TryGetValue(chatDto.Id, out var otherUserId) &&
                    displayNames.TryGetValue(otherUserId, out var displayName))
                {
                    chatDto.Name = displayName;
                }
            }
            return ApiResponse<List<ChatDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<ChatDto>> GetByIdAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat == null)
                return ApiResponse<ChatDto>.FailureResponse("Chat not found");

            if (!await _memberRepo.IsMemberAsync(chatId, userId))
                return ApiResponse<ChatDto>.FailureResponse("Unauthorized");

            var result = _mapper.Map<ChatDto>(chat);
            if (chat.Type == ChatType.Private)
            {
                var otherMemberIds = await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);
                if (otherMemberIds.TryGetValue(chatId, out var otherUserId))
                {
                    var displayNames = await _nameService.GetDisplayNamesAsync(userId,new[] { otherUserId });
                    if (displayNames.TryGetValue(otherUserId, out var displayName))
                        result.Name = displayName;
                }
            }
            return ApiResponse<ChatDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<ChatDto>> CreatePrivateAsync(Guid userId , CreatePrivateChatDto model)
        {
            if (userId == model.TargetId)
                return ApiResponse<ChatDto>.FailureResponse("Cannot create chat with yourself");
            var targetUser = await _userRepo.GetByIdAsync(model.TargetId);
            if (targetUser == null)
                return ApiResponse<ChatDto>.FailureResponse("User not found");
            if (await _blockedUserRepo.IsBlockedAsync(userId, model.TargetId))
                return ApiResponse<ChatDto>.FailureResponse(
                    "Cannot create chat with this user because a block exists");

            var existing = await _chatRepo.GetPrivateChatAsync(userId,model.TargetId);
            if (existing != null)
            {
                var existingDto = MapToDto(existing, userId);
                var existingNames = await _nameService.GetDisplayNamesAsync(
                    userId,new[] { model.TargetId });

                if (existingNames.TryGetValue(model.TargetId, out var existingName))
                    existingDto.Name = existingName;

                return ApiResponse<ChatDto>.SuccessResponse(existingDto);
            }
            var chat = _mapper.Map<Chat>(model);
            chat.ChatMembers = new List<ChatMember>
            {
                 new ChatMember { UserId = userId ,JoinedAt = DateTime.UtcNow },
                 new ChatMember { UserId = model.TargetId, JoinedAt = DateTime.UtcNow }
            };
            await _chatRepo.AddAsync(chat);
            var chatDto = MapToDto(chat, userId);
            var currentUserNames = await _nameService.GetDisplayNamesAsync(userId, new[] { model.TargetId });
            if (currentUserNames.TryGetValue(model.TargetId, out var currentUserName))
                chatDto.Name = currentUserName;

            var dtos = new Dictionary<Guid, ChatDto> { [userId] = chatDto };
            var targetChat = await GetByIdAsync(chat.Id, model.TargetId);
            if (targetChat.Success && targetChat.Data != null)
                dtos[model.TargetId] = targetChat.Data;

            await NotifyAsync(() => _notification.SendNewChatCreatedAsync(chat.Id, dtos));
            return ApiResponse<ChatDto>.SuccessResponse(chatDto);
        }
        public async Task<ApiResponse<ChatDto>> CreateGroupAsync(Guid userId, CreateGroupChatDto model)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<ChatDto>.FailureResponse("User not found");

            if (string.IsNullOrWhiteSpace(model.GroupName))
                return ApiResponse<ChatDto>.FailureResponse("Group name is required");

            if (model.MemberIds == null || !model.MemberIds.Any())
                return ApiResponse<ChatDto>.FailureResponse("Group must have at least one other member");

            var memberIds = model.MemberIds
                   .Where(id => id != userId && id != Guid.Empty)
                   .Distinct().ToList();

            var validUserIds = await _userRepo.GetExistingIdsAsync(memberIds, userId);
            if (validUserIds == null || !validUserIds.Any())
                return ApiResponse<ChatDto>.FailureResponse("No valid members were found");

            var chat = new Chat
            {
                Name = model.GroupName,
                Type = ChatType.Group,
                //GroupImage = model.,
                CreatedAt = DateTime.UtcNow,
                ChatMembers = new List<ChatMember>
            {
                new() { UserId = userId, IsAdmin = true , JoinedAt = DateTime.UtcNow }
            }
            };
            foreach (var memberId in validUserIds)
            {
                chat.ChatMembers.Add(new ChatMember
                {
                    UserId = memberId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                });
            }
            await _chatRepo.AddAsync(chat);
            var result = MapToDto(chat, userId);

            var chatPerUser = new Dictionary<Guid, ChatDto>();
            foreach (var memberId in validUserIds)
            {
                var response = await GetByIdAsync(chat.Id, memberId);
                if (response.Success && response.Data != null)
                    chatPerUser[memberId] = response.Data;
            }
            await NotifyAsync(() =>_notification.SendNewChatCreatedAsync(chat.Id, chatPerUser));
            return ApiResponse<ChatDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<string>> UpdateGroupAsync(Guid userId, UpdateGroupChatDto model)
        {
            var chat = await _chatRepo.GetByIdAsync(model.ChatId);
            if (chat == null)
                return ApiResponse<string>.FailureResponse("Chat not found");

            if (chat.Type != ChatType.Group)
                return ApiResponse<string>.FailureResponse("Not a group chat");

            if (!await _memberRepo.IsAdminAsync(model.ChatId, userId))
                return ApiResponse<string>.FailureResponse("Unauthorized");

            _mapper.Map(model, chat);
            await _chatRepo.UpdateAsync(chat);
            var result = MapToDto(chat, userId);
            await NotifyAsync(() => _notification.SendGroupUpdatedAsync(model.ChatId, result));

            return ApiResponse<string>.SuccessResponse("Group chat updated successfully");
        }
        public async Task<ApiResponse<string>> DeletePrivateAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat == null)
                return ApiResponse<string>.FailureResponse("Chat not found");

            if (chat.Type != ChatType.Private)
                return ApiResponse<string>.FailureResponse("Not a private chat");

            var member = await _memberRepo.GetMemberAsync(chatId, userId);
            if (member == null)
                return ApiResponse<string>.FailureResponse("Unauthorized");

            member.IsDeleted = true;
            member.RemovedAt = DateTime.UtcNow;
            await _memberRepo.UpdateAsync(member);
            return ApiResponse<string>.SuccessResponse("Chat deleted successfully");
        }
        public async Task<ApiResponse<string>> DeleteGroupAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat == null)
                return ApiResponse<string>.FailureResponse("Chat not found");

            if (chat.Type != ChatType.Group)
                return ApiResponse<string>.FailureResponse("Not a group");

            if (!await _memberRepo.IsAdminAsync(chatId, currentUserId))
                return ApiResponse<string>.FailureResponse("Only admin can delete group");

            var memberIds = await _memberRepo.GetMemberIdsAsync(chatId);
            await _chatRepo.DeleteAsync(chat);
            await NotifyAsync(() => _notification.SendChatDeletedAsync(chatId, memberIds));
            return ApiResponse<string>.SuccessResponse("Group deleted");
        }
        public async Task<ApiResponse<bool>> ArchiveChatAsync(Guid chatId, Guid userId)
        {
            var member = await _memberRepo.GetMemberAsync(chatId, userId);
            if (member == null)
                return ApiResponse<bool>.FailureResponse("Unauthorized");

            member.IsArchived = !member.IsArchived;
            await _memberRepo.UpdateAsync(member);
            return ApiResponse<bool>.SuccessResponse(member.IsArchived);
        }
        public async Task<ApiResponse<List<ChatDto>>> GetArchivedChatsAsync(Guid userId)
        {
            var archivedChats = await _chatRepo.GetArchivedChatsAsync(userId);
            if (archivedChats == null || !archivedChats.Any())
                return ApiResponse<List<ChatDto>>.FailureResponse("No archived chats found");
            var result = _mapper.Map<List<ChatDto>>(archivedChats,
                          opt => opt.Items["UserId"] = userId);
            var otherMemberIds = await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);
            var displayNames = await _nameService.GetDisplayNamesAsync(userId,otherMemberIds.Values);

            foreach (var chatDto in result)
            {
                if (otherMemberIds.TryGetValue(chatDto.Id, out var otherUserId) &&
                    displayNames.TryGetValue(otherUserId, out var displayName))
                {
                    chatDto.Name = displayName;
                }
            }
            return ApiResponse<List<ChatDto>>.SuccessResponse(
                _mapper.Map<List<ChatDto>>(result));
        }
        public async Task<ApiResponse<bool>> PinChatAsync(Guid chatId, Guid userId)
        {
            var member = await _memberRepo.GetMemberAsync(chatId, userId);
            if (member == null)
                return ApiResponse<bool>.FailureResponse("Unauthorized");
            member.IsPinned = !member.IsPinned;
            await _memberRepo.UpdateAsync(member);
            return ApiResponse<bool>.SuccessResponse(member.IsPinned);
        }
        public async Task<ApiResponse<List<ChatDto>>> GetPinnedChatsAsync(Guid userId)
        {
          var pinnedChats = await _chatRepo.GetPinnedChatsAsync(userId);
            if (pinnedChats == null || !pinnedChats.Any())
                return ApiResponse<List<ChatDto>>.FailureResponse("No pinned chats found");

            var result = _mapper.Map<List<ChatDto>>(
      pinnedChats,
      opt => opt.Items["UserId"] = userId);

            var otherMemberIds =
                await _memberRepo.GetPrivateChatOtherMemberIdsAsync(userId);
            var displayNames = await _nameService.GetDisplayNamesAsync(
        userId,
        otherMemberIds.Values);

            foreach (var chatDto in result)
            {
                if (otherMemberIds.TryGetValue(chatDto.Id, out var otherUserId) &&
                    displayNames.TryGetValue(otherUserId, out var displayName))
                {
                    chatDto.Name = displayName;
                }
            }

            //foreach (var chat in pinnedChats)
            //{
            //    if (chat.Type == ChatType.Private)
            //    {
            //        await _nameService.ApplyContactNamesAsync(pinnedChats, userId);
            //    }
            //}
            return ApiResponse<List<ChatDto>>.SuccessResponse(
                _mapper.Map<List<ChatDto>>(result));
        }

        #region Notification Methods
        public async Task<List<Guid>> GetChatIdsAsync(Guid userId)
            => await _chatRepo.GetChatIdsAsync(userId);
        public async Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId)
            => await _chatRepo.GetRelatedUserIdsAsync(userId);
        #endregion

        #region Helper Methods
        private ChatDto MapToDto(Chat chat, Guid userId)
            => _mapper.Map<ChatDto>(chat, opt => opt.Items["UserId"] = userId);

        private async Task NotifyAsync(Func<Task> send)
        {
            try { await send(); }
            catch (Exception ex) { logger.LogError(ex, "SignalR notification failed"); }
        }
        #endregion

    }
}

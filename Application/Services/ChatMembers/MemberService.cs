using Application.Interfaces;
using Application.Response;
using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats;
using Application.Services.Chats.Dtos;
using Application.Services.Contact.ContactName;
using Application.Services.Messages.Dtos;
using Application.Services.UserManagement.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services.ChatMembers
{  
    public class MemberService(
        IMemberRepository _memberRepo,
        IChatRepository _chatRepo,
        IUserRepository _userRepo,
        IChatService _chatService, 
        IContactNameService _nameService,
        IMapper _mapper, 
        IMemberNotificationService _notification, 
        ILogger<MemberService> logger) : IMemberService
    { 
        public async Task<ApiResponse<List<MembersDto>>> GetAllAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat == null)
                return ApiResponse<List<MembersDto>>.FailureResponse("Chat not found");

            if (!await _memberRepo.IsMemberAsync(chatId, currentUserId))
                return ApiResponse<List<MembersDto>>.FailureResponse("Access denied");

            var members = await _memberRepo.GetMembersAsync(chatId);
            var result = _mapper.Map<List<MembersDto>>(members);
            var displayNames = await _nameService.GetDisplayNamesAsync(currentUserId, members.Select(m => m.UserId).Distinct());
            foreach (var member in members)
            {
                if (displayNames.TryGetValue(member.UserId, out var displayName))
                {
                    var memberDto = result.FirstOrDefault(m =>m.UserName == member.User.Name);
                    if (memberDto != null)
                        memberDto.UserName = displayName;
                }
            }
            return ApiResponse<List<MembersDto>>.SuccessResponse(result);
              
        }
        public async Task<ApiResponse<string>> AddAsync(Guid currentUserId,AddMemberDto model)
        {
            var groupResult = await ValidateGroupAsync(model.ChatId, currentUserId);
            if (!groupResult.Success)
                return ApiResponse<string>.FailureResponse(groupResult.Message);

            if (!await _memberRepo.IsAdminAsync(model.ChatId, currentUserId))
                return ApiResponse<string>.FailureResponse("Only group admins can add members");

            if (model.UserId == null || !model.UserId.Any())
                return ApiResponse<string>.FailureResponse("Select at least one user");

            var requestedIds = model.UserId
                .Where(id => id != Guid.Empty && id != currentUserId)
                .Distinct()
                .ToList();

            var validUserIds = await _userRepo.GetExistingIdsAsync(requestedIds, currentUserId);
            var existingIds = await _memberRepo.GetExistingMemberIdsAsync(model.ChatId, validUserIds);

            var newMembers = validUserIds
                .Distinct()
                .Where(id => !existingIds.Contains(id))
                .Select(id => new ChatMember
                {
                    Id = Guid.NewGuid(),
                    ChatId = model.ChatId,
                    UserId = id,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                })
                .ToList();

            if (!newMembers.Any())
                return ApiResponse<string>.FailureResponse("No new valid members to add");

            await _memberRepo.AddMembersAsync(newMembers);
            var chatResponse = await _chatService.GetByIdAsync(model.ChatId, currentUserId);
            if (chatResponse.Success && chatResponse.Data != null)
            {
                foreach (var member in newMembers)
                {
                    await NotifyAsync(() =>_notification.SendMemberAddedAsync(
                            model.ChatId,
                            chatResponse.Data,
                            member.UserId));
                }
            }

            return ApiResponse<string>.SuccessResponse("Members added successfully");
        }
        public async Task<ApiResponse<string>> LeaveGroupAsync(Guid chatId, Guid userId)
        {
            var groupResult = await ValidateGroupAsync(chatId, userId, requireMember: true);
            if (!groupResult.Success)
                return ApiResponse<string>.FailureResponse(groupResult.Message);

            var memberResult = await ValidateMemberAsync(chatId, userId);
            if (!memberResult.Success)
                return ApiResponse<string>.FailureResponse(memberResult.Message);

            if (memberResult.Data.IsAdmin)
            {
                var otherAdmin = await _memberRepo.GetOtherAdminAsync(chatId, userId);
                if (otherAdmin == null)
                {
                    await _chatRepo.DeleteAsync(groupResult.Data);
                    return ApiResponse<string>.SuccessResponse(null, "Group deleted as you were the last admin");
                }
            }
            memberResult.Data.IsDeleted = true;
            memberResult.Data.RemovedAt = DateTime.UtcNow;
            await _memberRepo.UpdateAsync(memberResult.Data);
            await NotifyAsync(() => _notification.SendMemberLeftAsync(chatId, userId));
            return ApiResponse<string>.SuccessResponse("Left group successfully");
        }
        public async Task<ApiResponse<string>> RemoveAsync(Guid currentUserId,MemberActionDto model)
        {
            var groupResult = await ValidateGroupAsync(model.ChatId,currentUserId,requireAdmin: true);
            if (!groupResult.Success)
                return ApiResponse<string>.FailureResponse(groupResult.Message);

            if (currentUserId == model.TargetUserId)
                return ApiResponse<string>.FailureResponse("Use leave group instead");
           
            var targetResult = await ValidateMemberAsync(model.ChatId, model.TargetUserId);
            if (!targetResult.Success)
                return ApiResponse<string>.FailureResponse(targetResult.Message);

            if (targetResult.Data.IsAdmin)
                return ApiResponse<string>.FailureResponse("Admins cannot remove other admins");

            targetResult.Data.IsDeleted = true;
            targetResult.Data.RemovedAt = DateTime.UtcNow;
            await _memberRepo.UpdateAsync(targetResult.Data);

            await _notification.SendMemberRemovedAsync(model.ChatId, model.TargetUserId);
            return ApiResponse<string>.SuccessResponse("Member removed successfully");
        }
        public async Task<ApiResponse<bool>> PromoteAsync(MemberActionDto model, Guid currentUserId)
        {
            var groupResult = await ValidateGroupAsync(model.ChatId, currentUserId ,requireAdmin: true);
            if (!groupResult.Success)
                return ApiResponse<bool>.FailureResponse(groupResult.Message);

            var targetResult = await ValidateMemberAsync(model.ChatId,model.TargetUserId);
            if (!targetResult.Success)
                return ApiResponse<bool>.FailureResponse(targetResult.Message);

            targetResult.Data.IsAdmin = true;
            await _memberRepo.UpdateAsync(targetResult.Data);
            await _notification.SendPromotedAsync(model.ChatId, model.TargetUserId);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<bool>> DemoteAsync(MemberActionDto model, Guid currentUserId)
        {
            var groupResult = await ValidateGroupAsync(model.ChatId, currentUserId, requireAdmin: true);
            if (!groupResult.Success)
                return ApiResponse<bool>.FailureResponse(groupResult.Message);
            if (currentUserId == model.TargetUserId)
                return ApiResponse<bool>.FailureResponse("You cannot remove your own admin role");

            var targetResult = await ValidateMemberAsync(model.ChatId, model.TargetUserId);
            if (!targetResult.Success)
                return ApiResponse<bool>.FailureResponse(targetResult.Message);

            if (!targetResult.Data.IsAdmin)
                return ApiResponse<bool>.FailureResponse("User is not an admin");

            targetResult.Data.IsAdmin = false;
            await _memberRepo.UpdateAsync(targetResult.Data);
            await _notification.SendDemotedAsync(model.ChatId, model.TargetUserId);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<string>> MuteChatAsync(Guid userId, MuteChatDto model)
        {
            var memberResult = await ValidateMemberAsync(model.ChatId,userId);
            if (!memberResult.Success)
                return ApiResponse<string>.FailureResponse(memberResult.Message);

            if (memberResult.Data.IsMuted == model.IsMuted)
                return ApiResponse<string>.FailureResponse(
                    model.IsMuted ? "Chat is already muted" : "Chat is already unmuted");

            memberResult.Data.IsMuted = model.IsMuted;
            await _memberRepo.UpdateAsync(memberResult.Data);
            return ApiResponse<string>.SuccessResponse(model.IsMuted ? "Chat muted" : "Chat unmuted");
        }
        public async Task<ApiResponse<string>> PinChatAsync(Guid userId, PinChatDto model)
        {
            var memberResult = await ValidateMemberAsync(model.ChatId, userId);
            if (!memberResult.Success)
                return ApiResponse<string>.FailureResponse(memberResult.Message);

            if (memberResult.Data.IsPinned == model.IsPinned)
                return ApiResponse<string>.FailureResponse(
                    model.IsPinned ? "Chat is already pinned" : "Chat is already unpinned");

            memberResult.Data.IsPinned = model.IsPinned;
            await _memberRepo.UpdateAsync(memberResult.Data);
            return ApiResponse<string>.SuccessResponse(model.IsPinned ? "Chat pinned" : "Chat unpinned");
        }
        public async Task<bool>IsMemberAsync(Guid chatId, Guid userId)
            => await _memberRepo.IsMemberAsync(chatId, userId);
       
        #region Private methods
        private async Task<ApiResponse<Chat>> ValidateGroupAsync(Guid chatId, Guid currentUserId,
        bool requireAdmin = false, bool requireMember = false)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat == null)
                return ApiResponse<Chat>.FailureResponse("Chat not found");

            if (chat.Type != ChatType.Group)
                return ApiResponse<Chat>.FailureResponse("This chat is not a group");

            if (requireMember && !await _memberRepo.IsMemberAsync(chatId, currentUserId))
                return ApiResponse<Chat>.FailureResponse("You are not a member of this group");

            if (requireAdmin && !await _memberRepo.IsAdminAsync(chatId, currentUserId))
                return ApiResponse<Chat>.FailureResponse("Only admins can perform this action");

            return ApiResponse<Chat>.SuccessResponse(chat);
        }
        private async Task<ApiResponse<ChatMember>> ValidateMemberAsync(Guid chatId, Guid userId)
        {
            var member = await _memberRepo.GetMemberAsync(chatId, userId);
            if (member == null || member.IsDeleted)
                return ApiResponse<ChatMember>.FailureResponse("Member not found");

            return ApiResponse<ChatMember>.SuccessResponse(member);
        }
        private async Task NotifyAsync(Func<Task> send)
        {
            try { await send(); }
            catch (Exception ex) { logger.LogError(ex, "SignalR notification failed"); }
        }

        #endregion
    }
}

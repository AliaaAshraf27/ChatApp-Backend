using Application.Response;
using Application.Services.Status.Dtos;
using AutoMapper;
using Domain.IRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Application.Services.File;
using Application.Services.Status.Viewer;
using Application.Interfaces;
using Application.Services.Contact.ContactName;

namespace Application.Services.Status
{
    public class StatusService(
        IStatusRepository _statusRepo, 
        IUserRepository _userRepo,
        IContactRepository _contactRepo, 
        IViewStatusRepository _viewRepo, 
        IStatusNotificationService _notification,
        IMapper _mapper, 
        IFileStorageService _fileStorage,
        IContactNameService _nameService,
        IBlockedUserRepository _blockedUserRepo) : IStatusService
    {
        public async Task<ApiResponse<string>> CreateStatuesAsync(Guid userId, CreateStatuesDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Content) && model.Media == null)
                return ApiResponse<string>.FailureResponse(
                    "Status must have content or media");

            var status = _mapper.Map<Domain.Entities.Status>(model);
            status.UserId = userId;
            status.CreatedAt = DateTime.UtcNow;
            status.ExpiresAt = DateTime.UtcNow.AddHours(24);

            if (model.Media == null)
            {
                status.Type = StatusType.Text;
            }
            else
            {
                if (model.Media.ContentType.StartsWith("image/"))
                    status.Type = StatusType.Image;

                else if (model.Media.ContentType.StartsWith("video/"))
                    status.Type = StatusType.Video;

                else
                    return ApiResponse<string>.FailureResponse("Invalid media file type");

                var folder = status.Type == StatusType.Video
                      ? "status-videos": "status-images";

                var uploadResult = await _fileStorage.UploadAsync(model.Media,folder);
                status.MediaUrl = uploadResult.Url;
            }

            await _statusRepo.AddAsync(status);
            var contactIds = await _contactRepo.GetContactUserIdsAsync(userId);
            var user = await _userRepo.GetByIdAsync(userId);
            if (user != null && contactIds.Any())
                await _notification.SendNewStatusAsync(userId,contactIds);

            return ApiResponse<string>.SuccessResponse("Status created successfully");
        }
        public async Task<ApiResponse<List<UserStatusDto>>> GetContactsStatusesAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<List<UserStatusDto>>.FailureResponse("User not found");

            var contactIds = await _contactRepo.GetContactUserIdsAsync(userId);
            if (!contactIds.Any())
                return ApiResponse<List<UserStatusDto>>.SuccessResponse(new List<UserStatusDto>());

            var blockedUserIds = await _blockedUserRepo.GetBlockedUserIdsAsync(userId);
            contactIds = contactIds.Where(id => !blockedUserIds.Contains(id)).ToList();
            if (!contactIds.Any())
                return ApiResponse<List<UserStatusDto>>.SuccessResponse(new List<UserStatusDto>());

            var statuses = await _statusRepo.GetActiveStatusesAsync(userId, contactIds);
            if (!statuses.Any())
                return ApiResponse<List<UserStatusDto>>.SuccessResponse(new List<UserStatusDto>());
           
            var viewedStatusIds = await _statusRepo.GetViewedStatusIdsAsync(userId, statuses);

            var statusDtos = _mapper.Map<List<UserStatusDto>>(statuses);
            var displayNames = await _nameService.GetDisplayNamesAsync
                (userId, statuses.Select(s => s.UserId));
            foreach (var statusDto in statusDtos)
            {
                statusDto.IsViewed = viewedStatusIds.Contains(statusDto.StatusId);
                if (displayNames.TryGetValue(statusDto.UserId, out var displayName))
                    statusDto.UserName = displayName;
            }

            return ApiResponse<List<UserStatusDto>>.SuccessResponse(statusDtos);


        }
        public async Task<ApiResponse<List<MyStatusDto>>> GetMyStatusesAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<List<MyStatusDto>>.FailureResponse("User not found");

            var statuses = await _statusRepo.GetMyStatusesAsync(userId);
            if (!statuses.Any())
                return ApiResponse<List<MyStatusDto>>.SuccessResponse(new List<MyStatusDto>());

            return ApiResponse<List<MyStatusDto>>.SuccessResponse(_mapper.Map<List<MyStatusDto>>(statuses));

        }
        public async Task<ApiResponse<bool>> ViewStatusAsync(Guid userId, Guid statusId)
        {
            var status = await _statusRepo.GetByIdAsync(statusId);
            if (status == null || status.ExpiresAt <= DateTime.UtcNow)
                return ApiResponse<bool>.FailureResponse("Status not found");

            if (status.UserId == userId)
                return ApiResponse<bool>.FailureResponse("Cannot mark your own status as viewed");

            if (await _blockedUserRepo.IsBlockedAsync(userId, status.UserId))
                return ApiResponse<bool>.FailureResponse("Cannot view status because a block exists");

            var contactIds = await _contactRepo.GetContactUserIdsAsync(status.UserId);
            if (!contactIds.Contains(userId))
                return ApiResponse<bool>.FailureResponse("You cannot view this status");

            var alreadyViewed = await _viewRepo.HasViewedAsync(statusId, userId);
            if (alreadyViewed)
                return ApiResponse<bool>.SuccessResponse(true);

            var statusView = new StatusView
            {
                StatusId = statusId,
                ViewerId = userId,
                ViewedAt = DateTime.UtcNow
            };
            await _viewRepo.AddAsync(statusView);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<bool>> DeleteStatusAsync(Guid userId, Guid statusId)
        {
            var status = await _statusRepo.GetByIdAsync(statusId);
            if (status == null)
                return ApiResponse<bool>.FailureResponse("Status not found");
            if (status.UserId != userId)
                return ApiResponse<bool>.FailureResponse("You can only delete your own statuses");
            await _statusRepo.DeleteAsync(status);
            return ApiResponse<bool>.SuccessResponse(true);
        }
        public async Task<ApiResponse<List<StatusViewDto>>> GetViewersAsync(Guid userId, Guid statusId)
        {
            var status = await _statusRepo.GetByIdAsync(statusId);
            if (status == null)
                return ApiResponse<List<StatusViewDto>>.FailureResponse("Status not found");

            if (status.UserId != userId)
                return ApiResponse<List<StatusViewDto>>
                    .FailureResponse("You can only view viewers of your own statuses");

            var viewers = await _viewRepo.GetStatusViewersAsync(statusId);
            var viewerDtos = _mapper.Map<List<StatusViewDto>>(viewers);
            return ApiResponse<List<StatusViewDto>>.SuccessResponse(viewerDtos);
        }
    }
}

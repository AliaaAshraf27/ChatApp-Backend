using Application.Interfaces;
using Application.Response;
using Application.Services.Auth.Dtos;
using Application.Services.ChatMembers;
using Application.Services.Contact.ContactName;
using Application.Services.Contact.Dtos;
using Application.Services.UserManagement.Dtos;
using AutoMapper;
using AutoMapper.Execution;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserManagement
{
    public class UserService(
        IUserRepository _userRepo,
        IContactNameService _nameService, 
        IMapper _mapper) : IUserService
    {
        public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid Id)
        {
           var user = await _userRepo.GetByIdAsync(Id);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("User not found");
            return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user));
        }
        public async Task<ApiResponse<UserDto>> GetUserProfileAsync(Guid targetUserId,Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(targetUserId);
            if (user == null || !user.IsVerified)
                return ApiResponse<UserDto>.FailureResponse("User not found");
            var result = _mapper.Map<UserDto>(user);

            var displayNames = await _nameService.GetDisplayNamesAsync(
                userId, new[] { targetUserId });

            if (displayNames.TryGetValue(targetUserId, out var displayName))
                result.Name = displayName;

            return ApiResponse<UserDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<UserDto>> UpdateProfileAsync(Guid Id , ProfileDto model)
        {
            var user = await _userRepo.GetByIdAsync(Id);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("User not found");

            _mapper.Map(model, user);
            await _userRepo.UpdateAsync(user);
            var result = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResponse(result);
        }
        public async Task<ApiResponse<List<SearchDto>>> SearchAsync(FilterDto model,Guid userId)
        {
            if (string.IsNullOrWhiteSpace(model.PhoneNumber) && string.IsNullOrWhiteSpace(model.Name))
                return ApiResponse<List<SearchDto>>.FailureResponse("Provide at least a name or phone number");

            var users = await _userRepo.SearchAsync(
                model.PhoneNumber, model.Name, model.PageNumber, model.PageSize, userId);
            if (!users.Any())
                return ApiResponse<List<SearchDto>>.FailureResponse("No users found");
            
            var contactNames = await _nameService.GetDisplayNamesAsync(userId,users.Select(u => u.Id));
            foreach (var user in users)
            {
                if (contactNames.TryGetValue(user.Id, out var contactName))
                {
                    user.Name = contactName;
                }
            }
            var result = _mapper.Map<List<SearchDto>>(users);
            return ApiResponse<List<SearchDto>>.SuccessResponse(result);
        }
        public async Task SetOnlineAsync(Guid userId)
            => await _userRepo.SetOnlineAsync(userId);
        public async Task<DateTime> SetOfflineAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            await _userRepo.SetOfflineAsync(userId, now);
            return now;
        }
        
       
    }
}

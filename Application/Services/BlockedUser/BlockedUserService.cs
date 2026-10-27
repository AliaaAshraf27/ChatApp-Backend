using Application.Response;
using Application.Services.BlockedUser.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;

namespace Application.Services.BlockedUser
{
    public class BlockedUserService(
        IBlockedUserRepository _blockedUserRepo ,
        IUserRepository _userRepo,
        IMapper _mapper) : IBlockedUserService
    {
        public async Task<ApiResponse<bool>> BlockUserAsync(Guid userId, BlockUserDto model)
        {
            if (userId == model.BlockedId)
                return ApiResponse<bool>.FailureResponse("You cannot block yourself.");

            var targetUser = await _userRepo.GetByIdAsync(model.BlockedId);
            if (targetUser == null)
                return ApiResponse<bool>.FailureResponse("Target user does not exist.");

            var isBlocked = await _blockedUserRepo.IsBlockedAsync(userId, model.BlockedId);
            if (isBlocked)
                return ApiResponse<bool>.FailureResponse("User is already blocked.");

            var existingBlock = await _blockedUserRepo.GetAsync(userId, model.BlockedId);
            if (existingBlock != null)
                return ApiResponse<bool>.FailureResponse("User is already blocked.");

            var block = _mapper.Map<UserBlock>(model);
            block.UserId = userId;
            block.BlockedUserId = model.BlockedId;
            await _blockedUserRepo.AddAsync(block);
            return ApiResponse<bool>.SuccessResponse(true, "User blocked successfully.");
        }
        public async Task<ApiResponse<bool>> UnBlockUserAsync(Guid userId, BlockUserDto model)
        {
            if (userId == model.BlockedId)
                return ApiResponse<bool>.FailureResponse("You cannot unblock yourself.");

            var existing = await _blockedUserRepo.GetAsync(userId, model.BlockedId);
            if (existing == null)
                return ApiResponse<bool>.FailureResponse("User is not blocked");

            await _blockedUserRepo.DeleteAsync(existing);
            return ApiResponse<bool>.SuccessResponse(true);

        }
        public async Task<ApiResponse<List<BlockedUserDto>>> GetBlockedUsersAsync(Guid userId)
        {
            var blockedList = await _blockedUserRepo.GetBlockedListAsync(userId);
            if (blockedList == null || !blockedList.Any())
                return ApiResponse<List<BlockedUserDto>>.FailureResponse("No blocked users found.");

            var result = blockedList.Select(x => new BlockedUserDto
                {
                    PhoneNumber = x.BlockedUser.PhoneNumber
                }).ToList();

            return ApiResponse<List<BlockedUserDto>>.SuccessResponse(result);
        }
    }
}

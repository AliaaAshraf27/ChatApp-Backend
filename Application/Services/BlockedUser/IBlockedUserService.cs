using Application.Response;
using Application.Services.BlockedUser.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.BlockedUser
{
    public interface IBlockedUserService
    {
        Task<ApiResponse<bool>> BlockUserAsync(Guid userId, BlockUserDto model);
        Task<ApiResponse<bool>> UnBlockUserAsync(Guid userId, BlockUserDto model);
        Task<ApiResponse<List<BlockedUserDto>>> GetBlockedUsersAsync(Guid userId);
    }
}

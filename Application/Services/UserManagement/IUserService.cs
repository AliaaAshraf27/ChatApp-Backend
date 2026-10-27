using Application.Response;
using Application.Services.Auth.Dtos;
using Application.Services.Contact.Dtos;
using Application.Services.UserManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UserManagement
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetByIdAsync(Guid Id);
        Task<ApiResponse<UserDto>> GetUserProfileAsync(Guid targetUserId,Guid userId);
        Task<ApiResponse<UserDto>> UpdateProfileAsync(Guid Id, ProfileDto model);
        Task<ApiResponse<List<SearchDto>>> SearchAsync(FilterDto model,Guid userId);
        Task SetOnlineAsync(Guid userId);
        Task<DateTime> SetOfflineAsync(Guid userId);

    }
}

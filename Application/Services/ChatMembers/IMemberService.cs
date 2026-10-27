using Application.Response;
using Application.Services.ChatMembers.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ChatMembers
{
    public interface IMemberService
    {
        Task<ApiResponse<List<MembersDto>>> GetAllAsync(Guid chatId, Guid currentUserId);
        Task<ApiResponse<string>> AddAsync(Guid currentUserId, AddMemberDto model);
        Task<ApiResponse<string>> LeaveGroupAsync(Guid chatId, Guid userId);
        Task<ApiResponse<string>> RemoveAsync(Guid currentUserId, MemberActionDto model);
        Task<ApiResponse<bool>> PromoteAsync(MemberActionDto model, Guid currentUserId);
        Task<ApiResponse<bool>> DemoteAsync(MemberActionDto model, Guid currentUserId);
        Task<ApiResponse<string>> MuteChatAsync(Guid userId, MuteChatDto model);
        Task<ApiResponse<string>> PinChatAsync(Guid userId, PinChatDto model);
        Task<bool> IsMemberAsync(Guid chatId, Guid userId);
    }
}

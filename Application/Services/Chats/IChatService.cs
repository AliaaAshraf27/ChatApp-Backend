using Application.Response;
using Application.Services.ChatMembers.Dtos;
using Application.Services.Chats.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chats
{
    public interface IChatService
    {
        Task<ApiResponse<List<ChatDto>>> GetUserChatsAsync(Guid userId);
        Task<ApiResponse<ChatDto>> GetByIdAsync(Guid chatId, Guid userId);
        Task<ApiResponse<List<ChatDto>>> GetArchivedChatsAsync(Guid userId);
        Task<ApiResponse<List<ChatDto>>> GetPinnedChatsAsync(Guid userId);
        Task<ApiResponse<ChatDto>> CreatePrivateAsync(Guid userId, CreatePrivateChatDto model);
        Task<ApiResponse<ChatDto>> CreateGroupAsync(Guid userId, CreateGroupChatDto model);
        Task<ApiResponse<string>> UpdateGroupAsync(Guid userId, UpdateGroupChatDto model);
        Task<ApiResponse<string>> DeletePrivateAsync(Guid userId, Guid chatId);
        Task<ApiResponse<string>> DeleteGroupAsync(Guid chatId, Guid currentUserId);
        Task<ApiResponse<bool>> ArchiveChatAsync(Guid chatId, Guid userId);
        Task<ApiResponse<bool>> PinChatAsync(Guid chatId, Guid userId);
       Task<List<Guid>> GetChatIdsAsync(Guid userId);
        Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId);



    }
}

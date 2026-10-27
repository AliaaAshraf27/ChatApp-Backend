using Application.Helpers;
using Application.Response;
using Application.Services.Messages.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Messages
{
    public interface IMessageService
    {
        Task<ApiResponse<PagedResult<MessageDto>>> GetChatMessagesAsync(MessageFilterDto dto, Guid userId);
        Task<ApiResponse<MessageDto>> GetByIdAsync(Guid messageId, Guid userId);
        Task<ApiResponse<MessageDto>> SendMessageAsync(Guid userId, SendMessageDto model);
        Task<ApiResponse<MessageDto>> UpdateMessageAsync(Guid userId, UpdateMessageDto model);
        Task<ApiResponse<string>> DeleteMessageAsync(Guid userId, MessageActionDto model);
        Task<ApiResponse<string>> MarkAsReadAsync(MessageActionDto model, Guid userId);
        Task<ApiResponse<string>> MarkAsDeliveredAsync(Guid userId, Guid chatId, Guid lastMessageId);
        Task<ApiResponse<List<ResponseSearchDto>>> SearchAsync(SearchMessageDto model);
        Task<ApiResponse<bool>> ForwardMessageAsync(Guid userId, ForwardMessageDto model);
        Task<ApiResponse<List<MessageDto>>> GetUnreadMessagesAsync(Guid userId);
    }
}

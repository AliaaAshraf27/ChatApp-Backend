using Application.Services.Messages.Dtos;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageNotificationService
    {
        Task SendMessageAsync(MessageDto model);
        Task SendMessageUpdatedAsync(MessageDto model);
        Task SendMessageDeletedAsync(Guid chatId, Guid messageId);
        Task SendMessagesDeliveredAsync(Guid chatId, Guid lastMessageId, Guid deliveredToUserId);
        Task SendMessagesReadAsync(Guid chatId, Guid lastMessageId, Guid readByUserId);
    }
}

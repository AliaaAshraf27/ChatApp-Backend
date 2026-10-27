using Application.Services.Chats.Dtos;
using Application.Services.UserManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Interfaces
{
    public interface IMemberNotificationService
    {
        Task SendMemberAddedAsync(Guid chatId, ChatDto model, Guid userId);
        Task SendMemberRemovedAsync(Guid chatId, Guid userId);
        Task SendMemberLeftAsync(Guid chatId, Guid userId);
        Task SendPromotedAsync(Guid chatId, Guid userId);
        Task SendDemotedAsync(Guid chatId, Guid userId);
       // Task SendProfileUpdatedAsync(UserDto userDto, IEnumerable<Guid> relatedUserIds);

        //Task SendMemberAddedAsync(Guid chatId, ChatDto model, Guid userId);
        //Task SendMemberRemovedAsync(Guid chatId, Guid userId);
        //Task SendMemberLeftAsync(Guid chatId, Guid userId);
        //Task SendPromotedAsync(Guid chatId, Guid userId);
        //Task SendDemotedAsync(Guid chatId, Guid userId);
        //Task SendProfileUpdatedAsync(Guid userId, UserDto userDto);
    }
}

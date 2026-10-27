using Application.Services.Messages.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Contact.ContactName
{
    public interface IContactNameService
    {
        Task<Dictionary<Guid, string>> GetDisplayNamesAsync
            (Guid currentUserId, IEnumerable<Guid> userIds);
        Task ApplyContactNamesAsync(List<Chat> chats, Guid userId);
        Task ApplySenderNamesAsync(IEnumerable<SenderDto> senders, Guid userId);
    }
}

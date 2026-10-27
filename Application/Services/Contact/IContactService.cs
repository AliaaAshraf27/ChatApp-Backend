using Application.Response;
using Application.Services.Contact.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Contact
{
    public interface IContactService
    {
        Task<ApiResponse<List<ContactDto>>> GetContactsAsync(Guid userId);
        Task<ApiResponse<bool>> AddContactAsync(Guid userId, CreateContactDto model);
        Task<ApiResponse<bool>> DeleteContactAsync(Guid userId, Guid contactUserId);
        Task<Dictionary<Guid, string>> GetContactNamesAsync(
           Guid userId,IEnumerable<Guid> contactUserIds);
    }
}

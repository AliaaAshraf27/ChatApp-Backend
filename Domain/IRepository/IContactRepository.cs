using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IContactRepository
    {
        Task<List<Guid>> GetUsersWhoSavedMeAsync(Guid userId);
        Task<List<Contact>> GetContactsAsync(Guid userId);
        Task<List<Guid>> GetContactUserIdsAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId, Guid contactUserId);
        Task AddAsync(Contact contact);
        Task DeleteAsync(Guid userId, Guid contactUserId);
        Task<Dictionary<Guid, string>> GetContactNamesAsync(Guid userId,IEnumerable<Guid> contactUserIds);
        Task<string?> GetContactNameAsync(Guid userId, Guid contactUserId);
    }
}

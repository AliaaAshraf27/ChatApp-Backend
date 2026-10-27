using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByPhoneAsync(string phoneNumber);
        Task<User?> GetByIdAsync(Guid id);
        Task<Dictionary<Guid, string>> GetNamesByIdsAsync(
     IEnumerable<Guid> userIds);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<List<User>> SearchAsync(string? phoneNumber, string? name, int page, int pageSize, Guid userId);
        Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids, Guid excludeId);
        Task SetOnlineAsync(Guid userId);
        Task SetOfflineAsync(Guid userId, DateTime at);
    }
}

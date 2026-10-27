using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IBlockedUserRepository
    {
        Task<UserBlock?> GetAsync(Guid userId, Guid blockedId);
        Task<List<UserBlock>> GetBlockedListAsync(Guid userId);
        Task<List<Guid>> GetBlockedUserIdsAsync(Guid userId);
        Task<bool> IsBlockedAsync(Guid userId, Guid blockedId);
        Task AddAsync(UserBlock block);
        Task DeleteAsync(UserBlock user);
    }
}

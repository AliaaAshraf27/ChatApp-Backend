using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IStatusRepository
    {
        Task<Status?> GetByIdAsync(Guid id);
        Task<List<Status>> GetMyStatusesAsync(Guid userId);
        Task<List<Status>> GetActiveStatusesAsync(Guid userId, List<Guid> contactIds);
        Task<List<Guid>> GetViewedStatusIdsAsync(Guid viewerId, List<Status> statuses);
        Task AddAsync(Status status);
        Task DeleteAsync(Status status);
    }
}

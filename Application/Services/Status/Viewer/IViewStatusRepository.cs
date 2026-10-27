using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status.Viewer
{
    public interface IViewStatusRepository
    {
        Task<bool> HasViewedAsync(Guid statusId, Guid userId);
        Task<List<StatusView>> GetStatusViewersAsync(Guid statusId);
        Task AddAsync(StatusView viewer);
    }
}

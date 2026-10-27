using Domain.Entities;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class StatusRepository(ApplicationDbContext _context) : IStatusRepository
    {
        public async Task<Status?> GetByIdAsync(Guid id)
            => await _context.Statuses
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        public async Task<List<Status>> GetMyStatusesAsync(Guid userId)
            => await _context.Statuses
                .AsNoTracking()
                .Where(s => s.UserId == userId &&
                            s.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        public async Task<List<Status>> GetActiveStatusesAsync(Guid userId, List<Guid> contactIds)
            => await _context.Statuses
                .AsNoTracking()
                .Include(s => s.User)
                .Where(s => contactIds.Contains(s.UserId) &&
                       s.ExpiresAt > DateTime.UtcNow)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();
        public async Task<List<Guid>> GetViewedStatusIdsAsync(Guid viewerId, List<Status> statuses)
        {
            var statusIds = statuses.Select(x => x.Id).ToList();
            return await _context.StatusViews
                .Where(x =>
                    x.ViewerId == viewerId &&
                    statusIds.Contains(x.StatusId))
                .Select(x => x.StatusId)
                .Distinct()
                .ToListAsync();
        }
        public async Task AddAsync(Status status)
        {
            await _context.Statuses.AddAsync(status);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Status status)
        {
            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
        }
       

    }
}

using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status.Viewer
{
    public class ViewStatusRepository(ApplicationDbContext _context) : IViewStatusRepository
    {
        public async Task<bool> HasViewedAsync(Guid statusId, Guid userId)
           => await _context.StatusViews
               .AnyAsync(x => x.StatusId == statusId && x.ViewerId == userId);
        public async Task<List<StatusView>> GetStatusViewersAsync(Guid statusId)
            => await _context.StatusViews
                .Include(v => v.Viewer)
                .Where(x => x.StatusId == statusId)
                .OrderByDescending(x => x.ViewedAt)
                .ToListAsync();
        
        public async Task AddAsync(StatusView viewer)
        {
            await _context.StatusViews.AddAsync(viewer);
            await _context.SaveChangesAsync();
        }
    }
}

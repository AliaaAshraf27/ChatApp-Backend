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
    public class BlockedUserRepository(ApplicationDbContext _context) : IBlockedUserRepository
    {
        public async Task<List<UserBlock>> GetBlockedListAsync(Guid userId)
            => await _context.UserBlocks
                .Include(b => b.BlockedUser)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        public async Task<UserBlock?> GetAsync(Guid userId, Guid blockedId)
            => await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BlockedUserId == blockedId);
        public async Task<List<Guid>> GetBlockedUserIdsAsync(Guid userId)
            => await _context.UserBlocks
                .Where(b => b.UserId == userId || b.BlockedUserId == userId)
                .Select(b => b.UserId == userId
                    ? b.BlockedUserId
                    : b.UserId)
                .Distinct()
                .ToListAsync();
        public async Task<bool> IsBlockedAsync(Guid userId, Guid targetUserId)
            => await _context.UserBlocks.AnyAsync(b =>
                    (b.UserId == userId && b.BlockedUserId == targetUserId) ||
                    (b.UserId == targetUserId && b.BlockedUserId == userId));
        public async Task AddAsync(UserBlock block)
        {
           await _context.UserBlocks.AddAsync(block);
           await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(UserBlock user)
        {
            _context.UserBlocks.Remove(user);
            await _context.SaveChangesAsync();
        }

    }
}

using Domain.IRepository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure.Repository
{
    public class UserRepository(ApplicationDbContext _context) : IUserRepository
    {
        public async Task<User?> GetByPhoneAsync(string PhoneNumber)
            => await _context.Users.SingleOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
        public async Task<User?> GetByIdAsync(Guid id) 
            => await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        public async Task<Dictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> userIds)
        {
            var ids = userIds.Distinct().ToList();
            return await _context.Users.AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task<List<User>> SearchAsync(string? phoneNumber,string? name,int page,int pageSize,Guid userId)
        {
            var query = _context.Users.AsNoTracking()
                .Where(u => u.IsVerified && u.Id != userId);
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                query = query.Where(u => u.PhoneNumber == phoneNumber);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => u.Name.Contains(name) ||
                    _context.Contacts.Any(c =>
                        c.UserId == userId &&
                        c.ContactUserId == u.Id &&
                        c.Name != null &&
                        c.Name.Contains(name)));
            }
            return await query
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(Math.Min(pageSize, 50))
                .ToListAsync();
        }
        public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids, Guid excludeId)
            => await _context.Users
                .Where(u => ids.Contains(u.Id) && u.Id != excludeId) 
                .Select(u => u.Id)
                .ToListAsync();
        public Task SetOnlineAsync(Guid userId)
            => _context.Users.Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsOnline, true));
        public Task SetOfflineAsync(Guid userId, DateTime at)
            => _context.Users.Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IsOnline, false)
                    .SetProperty(u => u.LastSeen, at));

    }
}

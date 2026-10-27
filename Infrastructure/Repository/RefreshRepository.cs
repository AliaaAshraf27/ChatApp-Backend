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
    public class RefreshRepository(ApplicationDbContext context) : IRefreshRepository
    {
        public async Task<string> CreateAsync(Guid userId)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsExpired = false
            };
            await context.RefreshTokens.AddAsync(token);
            await context.SaveChangesAsync();

            return token.Token;
        }
        public async Task<RefreshToken?> GetActiveTokenAsync(string token)
            => await context.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == token &&
                !t.IsExpired &&
                t.ExpiresAt > DateTime.UtcNow);
        public async Task UpdateAsync(RefreshToken token)
        {
            context.RefreshTokens.Update(token);
            await context.SaveChangesAsync();
        }
    }
}

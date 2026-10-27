using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IRefreshRepository
    {
        Task<string> CreateAsync(Guid userId);
        Task<RefreshToken?> GetActiveTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
        
    }
}

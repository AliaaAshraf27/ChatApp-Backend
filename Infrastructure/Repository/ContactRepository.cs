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
    public class ContactRepository(ApplicationDbContext _context) : IContactRepository
    {
       public async Task<List<Guid>> GetUsersWhoSavedMeAsync(Guid userId)
            => await _context.Contacts
                .Where(c => c.UserId == userId)
                .Select(c => c.ContactUserId)
                .ToListAsync();
        public async Task<List<Contact>> GetContactsAsync(Guid userId)
            => await _context.Contacts
                .Include(x => x.ContactUser)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        public async Task<List<Guid>> GetContactUserIdsAsync(Guid userId)
            => await _context.Contacts
                .Where(c => c.UserId == userId)
                .Select(c => c.ContactUserId)
                .ToListAsync();
        public async Task<bool> ExistsAsync(Guid userId, Guid contactUserId)
            =>await _context.Contacts
                 .AnyAsync(c => c.UserId == userId && c.ContactUserId == contactUserId);
        public async Task AddAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid userId,Guid contactUserId)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(x => x.UserId == userId &&x.ContactUserId == contactUserId);

            if (contact == null)
                return;

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
        public async Task<Dictionary<Guid, string>> GetContactNamesAsync(Guid userId,
             IEnumerable<Guid> contactUserIds)
        {
            var ids = contactUserIds.Distinct().ToList();
            return await _context.Contacts
                .AsNoTracking()
                .Where(c =>
                    c.UserId == userId &&
                    ids.Contains(c.ContactUserId) &&
                    !string.IsNullOrEmpty(c.Name))
                .Select(c => new { c.ContactUserId, Name = c.Name! })
                .ToDictionaryAsync(x => x.ContactUserId, x => x.Name);
        }
        public async Task<string?> GetContactNameAsync(Guid userId, Guid contactUserId)
            => await _context.Contacts
                .AsNoTracking()
                .Where(c =>
                    c.UserId == userId &&
                    c.ContactUserId == contactUserId &&
                    !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        

    }
}

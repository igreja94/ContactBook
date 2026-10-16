using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Abstractions;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ContactRepository(ApplicationDbContext context) : BaseRepository<Contact>(context), IContactRepository
    {
        public async Task<IEnumerable<Contact>> GetContactsByUserAsync(Guid userId)
        {
            return await DbSet.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task DeleteContactsByUserAsync(Guid userId, ICollection<Guid> contactIds)
        {
            var contactsToDelete = await DbSet
                .Where(c => c.UserId == userId && contactIds.Contains(c.Id))
                .ToListAsync();

            DbSet.RemoveRange(contactsToDelete);
            await Context.SaveChangesAsync();
        }
    }
}

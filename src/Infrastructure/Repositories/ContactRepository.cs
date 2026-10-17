using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ContactRepository(ApplicationDbContext context) : BaseRepository<Contact>(context), IContactRepository
    {
        public async Task<IEnumerable<Contact>> GetContactsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(c => c.UserId == userId).ToListAsync(cancellationToken);
        }
    }
}

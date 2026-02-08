using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Abstractions;

namespace Infrastructure.Repositories
{
    public class ContactRepository(ApplicationDbContext context) : BaseRepository<Contact>(context)
    {
    }
}

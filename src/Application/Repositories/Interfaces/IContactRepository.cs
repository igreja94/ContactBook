using Domain.Entities;

namespace Application.Repositories.Interfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetContactsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

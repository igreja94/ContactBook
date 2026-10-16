using Domain.Entities;

namespace Application.Repositories
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetContactsByUserAsync(Guid userId);
        Task DeleteContactsByUserAsync(Guid userId, ICollection<Guid> contactIds);

    }
}

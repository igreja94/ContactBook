using Application.DTO;

namespace Application.Services.Interfaces
{
    public interface IContactService : IService<ContactDto>
    {
        Task<IEnumerable<ContactDto>> GetContactsByUserId(Guid id);

        Task DeleteContactsByUserAsync(Guid userId, ICollection<Guid> contactIds);
    }
}

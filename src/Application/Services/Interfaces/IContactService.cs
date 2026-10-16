using Application.DTO;

namespace Application.Services.Interfaces
{
    public interface IContactService : IService<ContactDto>
    {
        Task<IEnumerable<ContactDto>> GetContactsByUserId(Guid id, CancellationToken cancellationToken = default);
    }
}

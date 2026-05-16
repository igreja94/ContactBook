using Application.DTO;

namespace Application.Services.Interfaces
{
    public interface IContactService : IService<ContactDto>
    {
        Task<IEnumerable<ContactDto>> GetContactsByUserId(string id);

        Task DeleteContactsByUserAsync(string userId, ICollection<string> contactIds);
    }
}

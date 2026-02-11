using Application.DTO;
using Application.Extensions;
using Application.Services.Abstractions;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
    public class ContactService(IContactRepository repository) : BaseService<Contact, ContactDto>(repository), IContactService
    {
        protected override ContactDto MapToDto(Contact model)
        {
            return model.ToDto();
        }

        protected override Contact ToModel(ContactDto dto)
        {
            return dto.ToModel();
        }

        public async Task<IEnumerable<ContactDto>> GetContactsByUserId(string id)
        {
            var guid = Guid.Parse(id);
            var contacts = await repository.GetContactsByUserAsync(guid);
            return contacts.Select(c => c.ToDto());
        }

        public Task DeleteContactsByUserAsync(string userId, ICollection<string> contactIds)
        {
            throw new NotImplementedException();
        }


    }
}

using Application.DTO;
using Application.Exceptions;
using Application.Mappers.Interfaces;
using Application.Repositories.Interfaces;
using Application.Services.Abstractions;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ContactService(IContactRepository repository, IUserRepository userRepository, IContactMapper mapper)
        : BaseService<Contact, ContactDto>(repository, mapper), IContactService
    {
        public async Task<IEnumerable<ContactDto>> GetContactsByUserId(Guid id, CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(id, cancellationToken);

            var contacts = await repository.GetContactsByUserAsync(id, cancellationToken);
            return contacts.Select(mapper.ToDto);
        }

        private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (!await userRepository.ExistsAsync(userId, cancellationToken))
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }
        }
    }
}

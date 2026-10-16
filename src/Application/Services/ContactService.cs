using Application.DTO;
using Application.Extensions;
using Application.Services.Abstractions;
using Application.Services.Interfaces;
using Domain.Entities;
using Application.Repositories;

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

		protected override void ApplyUpdate(Contact model, ContactDto dto)
		{
			model.FirstName = dto.FirstName ?? string.Empty;
			model.LastName = dto.LastName ?? string.Empty;
			model.DateOfBirth = dto.DateOfBirth ?? DateTime.MinValue;
			model.Email = dto.Email ?? string.Empty;
			model.PhoneNumber = dto.PhoneNumber ?? string.Empty;
		}

		public async Task<IEnumerable<ContactDto>> GetContactsByUserId(Guid id)
		{
			var contacts = await repository.GetContactsByUserAsync(id);
			return contacts.Select(c => c.ToDto());
		}

		public Task DeleteContactsByUserAsync(Guid userId, ICollection<Guid> contactIds)
		{
			ArgumentNullException.ThrowIfNull(contactIds);

			return repository.DeleteContactsByUserAsync(userId, contactIds);
		}


	}
}

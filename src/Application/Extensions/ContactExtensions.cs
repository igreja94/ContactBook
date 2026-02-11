using Application.DTO;
using Domain.Entities;

namespace Application.Extensions
{
    public static class ContactExtensions
    {
        public static ContactDto ToDto(this Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id.ToString(),
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DateOfBirth = contact.DateOfBirth,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber
            };
        }

        public static Contact ToModel(this ContactDto contactDto)
        {
            return new Contact
            {
                FirstName = contactDto.FirstName ?? string.Empty,
                LastName = contactDto.LastName ?? string.Empty,
                DateOfBirth = contactDto.DateOfBirth ?? DateTime.MinValue,
                Email = contactDto.Email ?? string.Empty,
                PhoneNumber = contactDto.PhoneNumber ?? string.Empty
            };
        }
    }
}

using Application.DTO;
using Application.Mappers.Interfaces;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers
{
    [Mapper]
    public partial class ContactMapper : IContactMapper
    {
        [MapperIgnoreSource(nameof(Contact.User))]
        public partial ContactDto ToDto(Contact model);

        [MapperIgnoreSource(nameof(ContactDto.Id))]
        [MapperIgnoreTarget(nameof(Contact.Id))]
        [MapperIgnoreTarget(nameof(Contact.User))]
        public partial Contact ToModel(ContactDto dto);

        // The owner is immutable once the contact is created.
        [MapperIgnoreSource(nameof(ContactDto.Id))]
        [MapperIgnoreSource(nameof(ContactDto.UserId))]
        [MapperIgnoreTarget(nameof(Contact.Id))]
        [MapperIgnoreTarget(nameof(Contact.UserId))]
        [MapperIgnoreTarget(nameof(Contact.User))]
        public partial void ApplyTo(ContactDto dto, [MappingTarget] Contact model);
    }
}

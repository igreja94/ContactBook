using Application.DTO;
using Domain.Entities;

namespace Application.Mappers.Interfaces
{
    public interface IContactMapper : IEntityMapper<Contact, ContactDto>
    {
    }
}

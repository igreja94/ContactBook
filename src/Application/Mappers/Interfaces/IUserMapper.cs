using Application.DTO;
using Domain.Entities;

namespace Application.Mappers.Interfaces
{
    public interface IUserMapper : IEntityMapper<User, UserDto>
    {
    }
}

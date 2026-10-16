using Application.DTO;
using Application.Extensions;
using Application.Services.Abstractions;
using Domain.Entities;
using Application.Repositories;

namespace Application.Services
{
    public class UserService(IUserRepository repository) : BaseService<User, UserDto>(repository)
    {
        protected override UserDto MapToDto(User model)
        {
            return model.ToDto();
        }

        protected override User ToModel(UserDto dto)
        {
            return dto.ToModel();
        }
    }
}

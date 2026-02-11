using Application.DTO;
using Application.Extensions;
using Application.Services.Abstractions;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
    public class UserService(IRepository<User> repository) : BaseService<User, UserDto>(repository)
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

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

        protected override void ApplyUpdate(User model, UserDto dto)
        {
            model.FirstName = dto.FirstName ?? string.Empty;
            model.LastName = dto.LastName ?? string.Empty;
            model.DateOfBirth = dto.DateOfBirth ?? DateTime.MinValue;
            model.Email = dto.Email ?? string.Empty;
            model.PhoneNumber = dto.PhoneNumber ?? string.Empty;
        }
    }
}

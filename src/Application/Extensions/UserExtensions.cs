using Application.DTO;
using Domain.Entities;

namespace Application.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public static User ToModel(this UserDto userDto)
        {
            return new User
            {
                FirstName = userDto.FirstName ?? string.Empty,
                LastName = userDto.LastName ?? string.Empty,
                DateOfBirth = userDto.DateOfBirth ?? DateTime.MinValue,
                Email = userDto.Email ?? string.Empty,
                PhoneNumber = userDto.PhoneNumber ?? string.Empty,
            };
        }
    }
}

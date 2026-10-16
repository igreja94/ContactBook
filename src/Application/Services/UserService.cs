using Application.DTO;
using Application.Mappers.Interfaces;
using Application.Repositories.Interfaces;
using Application.Services.Abstractions;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserService(IUserRepository repository, IUserMapper mapper)
        : BaseService<User, UserDto>(repository, mapper), IUserService
    {
    }
}

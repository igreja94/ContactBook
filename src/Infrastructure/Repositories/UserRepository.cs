using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Abstractions;

namespace Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
    {
    }
}

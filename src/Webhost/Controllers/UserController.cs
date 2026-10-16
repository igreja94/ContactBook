using Application.DTO;
using Application.Services.Interfaces;
using WebApi.Controllers.Abstractions;

namespace Webhost.Controllers
{
    public class UserController(IService<UserDto> service) : BaseController<UserDto>(service)
    {
    }
}

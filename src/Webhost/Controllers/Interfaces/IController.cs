using Application.DTO.Interfaces;

namespace Webhost.Controllers.Interfaces
{
    public interface IController<TDto> where TDto : IDto
    {

    }
}

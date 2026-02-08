using Application.DTO.Interfaces;

namespace API.Controllers.Interfaces
{
    public interface IController<TDto> where TDto : IDto
    {

    }
}

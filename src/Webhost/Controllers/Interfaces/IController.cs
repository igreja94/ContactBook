using Application.DTO.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers.Interfaces
{
    public interface IController<TDto>
        where TDto : IDto
    {
        Task<ActionResult<IEnumerable<TDto>>> GetAll();

        Task<ActionResult<TDto>> GetById(Guid id);

        Task<IActionResult> Create(TDto dto);

        Task<IActionResult> Update(TDto dto);

        Task<IActionResult> Delete(Guid id);
    }
}

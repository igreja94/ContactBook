using Application.DTO.Interfaces;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Webhost.Controllers.Interfaces;

namespace WebApi.Controllers.Abstractions
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TDto>(IService<TDto> service) : ControllerBase, IController<TDto>
        where TDto : IDto
    {
        private readonly IService<TDto> service = service;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            return Ok(await service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TDto>> GetById(Guid id)
        {
            var dto = await FindAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Create([FromBody] TDto dto)
        {
            var created = await service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Update([FromBody] TDto dto)
        {
            if (dto.Id is null || dto.Id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(dto.Id), "A valid Id is required.");
                return ValidationProblem(ModelState);
            }

            if (await FindAsync(dto.Id.Value) is null)
            {
                return NotFound();
            }

            await service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            if (await FindAsync(id) is null)
            {
                return NotFound();
            }

            await service.DeleteAsync(id);
            return NoContent();
        }

        // The service signals a missing entity with an InvalidOperationException.
        protected async Task<TDto?> FindAsync(Guid id)
        {
            try
            {
                return await service.GetByIdAsync(id);
            }
            catch (InvalidOperationException)
            {
                return default;
            }
        }
    }
}

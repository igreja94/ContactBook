using Application.DTO;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController(IContactService contactService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await contactService.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await contactService.GetByIdAsync(id, cancellationToken));
        }

        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await contactService.GetContactsByUserId(userId, cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ContactDto dto, CancellationToken cancellationToken)
        {
            var created = await contactService.AddAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] ContactDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id is null || dto.Id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(dto.Id), "A valid Id is required.");
                return ValidationProblem(ModelState);
            }

            await contactService.UpdateAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await contactService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

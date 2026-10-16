using Application.DTO;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Abstractions;

namespace Webhost.Controllers
{
    public class ContactController(IContactService contactService, IService<UserDto> userService)
        : BaseController<ContactDto>(contactService)
    {
        public override async Task<IActionResult> Create([FromBody] ContactDto dto)
        {
            if (dto.UserId is null || !await UserExistsAsync(dto.UserId.Value))
            {
                ModelState.AddModelError(nameof(dto.UserId), "UserId must reference an existing user.");
                return ValidationProblem(ModelState);
            }

            return await base.Create(dto);
        }

        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetByUser(Guid userId)
        {
            if (!await UserExistsAsync(userId))
            {
                return NotFound();
            }

            return Ok(await contactService.GetContactsByUserId(userId));
        }

        // Contact ids that do not belong to the user are ignored.
        [HttpDelete("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteByUser(Guid userId, [FromBody] List<Guid> contactIds)
        {
            if (contactIds.Count == 0)
            {
                ModelState.AddModelError(nameof(contactIds), "At least one contact id is required.");
                return ValidationProblem(ModelState);
            }

            if (!await UserExistsAsync(userId))
            {
                return NotFound();
            }

            await contactService.DeleteContactsByUserAsync(userId, contactIds);
            return NoContent();
        }

        private async Task<bool> UserExistsAsync(Guid userId)
        {
            try
            {
                await userService.GetByIdAsync(userId);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}

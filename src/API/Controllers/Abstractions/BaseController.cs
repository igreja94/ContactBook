using API.Controllers.Interfaces;
using Application.DTO.Interfaces;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Abstractions
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TDto>(IService<TDto> service) : Controller, IController<TDto> where TDto : IDto
    {
        protected readonly IService<TDto> _service = service;

        [HttpGet]
        public virtual ActionResult<IEnumerable<TDto>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public virtual ActionResult<TDto> GetById(int id)
        {
            var result = _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] TDto dto)
        {
            _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = 0 }, dto); // Simplified for example
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] TDto dto)
        {
            _service.Update(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
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
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public virtual ActionResult<TDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] TDto dto)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] TDto dto)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
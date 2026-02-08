using Application.DTO.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities.Interfaces;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.Abstractions
{
    public abstract class BaseService<TDto>(IRepository<IModel> repository) : IService<TDto> where TDto : IDto
    {
        public IRepository<IModel> Repository { get; set; } = repository;

        public virtual void Create(TDto dto)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public virtual TDto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

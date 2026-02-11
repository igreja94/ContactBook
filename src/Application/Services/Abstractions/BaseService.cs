using Application.DTO.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities.Interfaces;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.Abstractions
{
    public abstract class BaseService<TModel, TDto>(IRepository<TModel> repository) : IService<TDto>
        where TModel : IModel
        where TDto : IDto
    {

        protected abstract TDto MapToDto(TModel model);

        protected abstract TModel ToModel(TDto dto);

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var models = await repository.GetAllAsync();
            return models.Select(e => MapToDto(e));
        }

        public virtual async Task<TDto> GetByIdAsync(string id)
        {
            var guid = Guid.Parse(id);
            var model = await repository.GetByIdAsync(guid);
            return MapToDto(model);
        }

        public virtual async Task AddAsync(TDto dto)
        {
            var model = ToModel(dto);
            await repository.AddAsync(model);
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            var model = ToModel(dto);
            await repository.UpdateAsync(model);
        }

        public virtual async Task DeleteAsync(string id)
        {
            var guid = Guid.Parse(id);
            var model = await repository.GetByIdAsync(guid);
            await repository.DeleteAsync(model);
        }

    }
}

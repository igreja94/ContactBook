using Application.DTO.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities.Interfaces;
using Application.Repositories;

namespace Application.Services.Abstractions
{
    public abstract class BaseService<TModel, TDto>(IRepository<TModel> repository) : IService<TDto>
        where TModel : IModel
        where TDto : IDto
    {

        protected abstract TDto MapToDto(TModel model);

        protected abstract TModel ToModel(TDto dto);

        // Copies the updatable fields of the dto onto the tracked model, preserving fields the dto does not expose.
        protected abstract void ApplyUpdate(TModel model, TDto dto);

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var models = await repository.GetAllAsync();
            return models.Select(e => MapToDto(e));
        }

        public virtual async Task<TDto> GetByIdAsync(Guid id)
        {
            var model = await repository.GetByIdAsync(id);
            return MapToDto(model);
        }

        // Identity is always generated here, any Id supplied by the caller is ignored.
        public virtual async Task<TDto> AddAsync(TDto dto)
        {
            var model = ToModel(dto);
            model.Id = Guid.NewGuid();
            await repository.AddAsync(model);
            return MapToDto(model);
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var id = dto.Id ?? throw new ArgumentException("The dto must have an Id.", nameof(dto));
            var model = await repository.GetByIdAsync(id);
            ApplyUpdate(model, dto);
            await repository.UpdateAsync(model);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var model = await repository.GetByIdAsync(id);
            await repository.DeleteAsync(model);
        }

    }
}

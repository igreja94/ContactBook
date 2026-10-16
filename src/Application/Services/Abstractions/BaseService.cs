using Application.DTO.Interfaces;
using Application.Mappers.Interfaces;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities.Interfaces;

namespace Application.Services.Abstractions
{
    public abstract class BaseService<TModel, TDto>(IRepository<TModel> repository, IEntityMapper<TModel, TDto> mapper) : IService<TDto>
        where TModel : IModel
        where TDto : IDto
    {
        public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var models = await repository.GetAllAsync(cancellationToken);
            return models.Select(mapper.ToDto);
        }

        public virtual async Task<TDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await repository.GetByIdAsync(id, cancellationToken);
            return mapper.ToDto(model);
        }

        // Identity is always generated here, any Id supplied by the caller is ignored.
        public virtual async Task<TDto> AddAsync(TDto dto, CancellationToken cancellationToken = default)
        {
            var model = mapper.ToModel(dto);
            model.Id = Guid.NewGuid();
            await repository.AddAsync(model, cancellationToken);
            return mapper.ToDto(model);
        }

        public virtual async Task UpdateAsync(TDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var id = dto.Id ?? throw new ArgumentException("The dto must have an Id.", nameof(dto));
            var model = await repository.GetByIdAsync(id, cancellationToken);
            mapper.ApplyTo(dto, model);
            await repository.UpdateAsync(model, cancellationToken);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await repository.GetByIdAsync(id, cancellationToken);
            await repository.DeleteAsync(model, cancellationToken);
        }
    }
}

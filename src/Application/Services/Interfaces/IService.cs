using Application.DTO.Interfaces;

namespace Application.Services.Interfaces
{
    public interface IService<TDto>
        where TDto : IDto
    {
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<TDto> AddAsync(TDto dto, CancellationToken cancellationToken = default);

        Task UpdateAsync(TDto dto, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

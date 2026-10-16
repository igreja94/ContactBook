using Application.DTO.Interfaces;

namespace Application.Services.Interfaces
{
    public interface IService<TDto>
        where TDto : IDto
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(Guid id);
        Task<TDto> AddAsync(TDto dto);
        Task UpdateAsync(TDto dto);
        Task DeleteAsync(Guid id);
    }
}

using Application.DTO.Interfaces;

namespace Application.Services.Interfaces
{
    public interface IService<TDto>
        where TDto : IDto
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(string id);
        Task AddAsync(TDto dto);
        Task UpdateAsync(TDto dto);
        Task DeleteAsync(string id);
    }
}

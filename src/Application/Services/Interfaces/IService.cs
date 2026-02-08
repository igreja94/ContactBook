using Application.DTO.Interfaces;

namespace Application.Services.Interfaces
{
    public interface IService<TDto> where TDto : IDto
    {
        IEnumerable<TDto> GetAll();
        TDto GetById(int id);
        void Create(TDto dto);
        void Update(TDto dto);
        void Delete(int id);
    }
}

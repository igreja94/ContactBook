using Domain.Entities.Interfaces;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IRepository<TModel> where TModel : IModel
    {
        Task<IEnumerable<TModel>> GetAllAsync();
        ValueTask<TModel> GetByIdAsync(Guid id);
        Task AddAsync(TModel model);
        Task UpdateAsync(TModel model);
        Task DeleteAsync(TModel model);
    }
}

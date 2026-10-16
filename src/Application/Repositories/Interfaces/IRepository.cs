using Domain.Entities.Interfaces;

namespace Application.Repositories.Interfaces
{
    public interface IRepository<TModel>
        where TModel : IModel
    {
        Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default);

        ValueTask<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(TModel model, CancellationToken cancellationToken = default);

        Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);

        Task DeleteAsync(TModel model, CancellationToken cancellationToken = default);
    }
}

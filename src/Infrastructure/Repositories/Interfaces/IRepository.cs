using Domain.Entities.Interfaces;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IRepository<TModel> where TModel : IModel
    {
        Task<IEnumerable<TModel>> GetAll();
        Task<TModel> GetById(Guid id);
        Task Add(TModel model);
        Task Update(TModel model);
        Task Delete(TModel model);
    }
}

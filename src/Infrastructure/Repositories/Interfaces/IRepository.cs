using Domain.Entities.Interfaces;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IRepository<TModel> where TModel : IModel
    {
        IEnumerable<TModel> GetAll();
        TModel GetById(Guid id);
        void Add(TModel model);
        void Update(TModel model);
        void Delete(TModel model);
    }
}

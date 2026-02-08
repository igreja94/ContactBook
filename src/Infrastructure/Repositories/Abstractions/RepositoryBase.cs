using Domain.Entities.Interfaces;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Abstractions
{
    public abstract class RepositoryBase<TModel> : IRepository<TModel> where TModel : IModel
    {
        public void Add(TModel model)
        {
            throw new NotImplementedException();
        }

        public void Delete(TModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public TModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TModel model)
        {
            throw new NotImplementedException();
        }
    }
}

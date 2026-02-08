using Domain.Entities.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Abstractions
{
    public abstract class BaseRepository<TModel>(ApplicationDbContext context) : IRepository<TModel> where TModel : class, IModel
    {
        protected readonly ApplicationDbContext Context = context;
        protected readonly DbSet<TModel> DbSet = context.Set<TModel>();

        public virtual async Task<IEnumerable<TModel>> GetAll()
        {
            return [.. DbSet];
        }

        public virtual async Task<TModel> GetById(Guid id)
        {
            return DbSet.Find(id) ??
                throw new KeyNotFoundException($"Entity of type {typeof(TModel).Name} with ID {id} was not found.");
        }

        public virtual async Task Add(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Add(model);
            Context.SaveChanges();
        }

        public virtual async Task Update(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Update(model);
            Context.SaveChanges();
        }

        public virtual async Task Delete(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Remove(model);
            Context.SaveChanges();
        }
    }
}

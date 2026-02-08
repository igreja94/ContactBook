using Domain.Entities.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Abstractions
{
    public abstract class BaseRepository<TModel>(ApplicationDbContext context) : IRepository<TModel> where TModel : class, IModel
    {
        protected readonly ApplicationDbContext _context = context;
        protected readonly DbSet<TModel> _dbSet = context.Set<TModel>();

        public virtual void Add(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            _dbSet.Add(model);
            _context.SaveChanges();
        }

        public virtual void Delete(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            _dbSet.Remove(model);
            _context.SaveChanges();
        }

        public virtual IEnumerable<TModel> GetAll()
        {
            return [.. _dbSet];
        }

        public virtual TModel GetById(Guid id)
        {
            return _dbSet.Find(id) ??
                throw new KeyNotFoundException($"Entity of type {typeof(TModel).Name} with ID {id} was not found.");
        }

        public virtual void Update(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            _dbSet.Update(model);
            _context.SaveChanges();
        }
    }
}

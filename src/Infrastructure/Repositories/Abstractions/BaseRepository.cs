using Application.Repositories;
using Domain.Entities.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Abstractions
{
    public abstract class BaseRepository<TModel>(ApplicationDbContext context) : IRepository<TModel>
        where TModel : class, IModel
    {
        protected ApplicationDbContext Context { get; } = context;

        protected DbSet<TModel> DbSet { get; } = context.Set<TModel>();

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async ValueTask<TModel> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id) ?? throw new InvalidOperationException($"{typeof(TModel).Name} with id {id} not found.");
        }

        public async Task AddAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await DbSet.AddAsync(model);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Update(model);
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Remove(model);
            await Context.SaveChangesAsync();
        }
    }
}

using Application.Exceptions;
using Application.Repositories.Interfaces;
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

        public async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public async ValueTask<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync([id], cancellationToken) ?? throw new NotFoundException($"{typeof(TModel).Name} with id {id} not found.");
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return DbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(TModel model, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);

            await DbSet.AddAsync(model, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);

            // Tracked entities are already change-detected, Update is only needed to attach detached ones.
            if (Context.Entry(model).State == EntityState.Detached)
            {
                DbSet.Update(model);
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(model);

            DbSet.Remove(model);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

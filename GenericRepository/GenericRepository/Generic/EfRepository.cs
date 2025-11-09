using GenericRepository.Base_Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GenericRepository.Generic
{
    // Entity Framework implementation
    public class EfRepository<TEntity, TKey>(DbContext context) : IQueryableRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly DbContext _context = context;
        protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;

            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var entityList = entities.ToList();
            var now = DateTime.UtcNow;

            foreach (var entity in entityList)
            {
                entity.CreatedAt = now;
                entity.UpdatedAt = null;
            }

            await _dbSet.AddRangeAsync(entityList, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entityList;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                await DeleteAsync(entity, cancellationToken);
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await CountAsync(cancellationToken);
            var items = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(predicate);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual IQueryable<TEntity> Query()
        {
            return _dbSet;
        }

        public virtual IQueryable<TEntity> QueryAsNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
    }
}

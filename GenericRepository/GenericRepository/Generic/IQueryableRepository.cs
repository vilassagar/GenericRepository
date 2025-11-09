using GenericRepository.Base_Entity;
using System.Linq.Expressions;

namespace GenericRepository.Generic
{
    // Queryable repository interface for advanced querying
    public interface IQueryableRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> Query();
        IQueryable<TEntity> QueryAsNoTracking();

        // Default implementation for complex queries
        async Task<IEnumerable<TResult>> SelectAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return await Query().Select(selector).ToListAsync(cancellationToken);
        }

        async Task<IEnumerable<TResult>> SelectManyAsync<TResult>(
            Expression<Func<TEntity, IEnumerable<TResult>>> selector,
            CancellationToken cancellationToken = default)
        {
            return await Query().SelectMany(selector).ToListAsync(cancellationToken);
        }
    }
}

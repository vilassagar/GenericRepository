using GenericRepository.Base_Entity;
using GenericRepository.Generic;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.repository
{
    // User repository implementation
    public class UserRepository(DbContext context) : EfRepository<User, Guid>(context), IUserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersByEmailDomainAsync(string domain, CancellationToken cancellationToken = default)
        {
            var emailPattern = $"@{domain}";
            return await _dbSet
                .Where(u => u.Email.EndsWith(emailPattern))
                .OrderBy(u => u.Username)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync(DateTime since, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(u => u.UpdatedAt.HasValue && u.UpdatedAt.Value >= since)
                .OrderByDescending(u => u.UpdatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Where(u => u.Username.ToLower().Contains(term) ||
                           u.Email.ToLower().Contains(term) ||
                           (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                           (u.LastName != null && u.LastName.ToLower().Contains(term)))
                .OrderBy(u => u.Username)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default)
        {
            return !await _dbSet.AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<PagedResult<User>> GetUsersByCreationDateAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

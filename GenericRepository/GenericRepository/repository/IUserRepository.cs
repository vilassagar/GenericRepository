using GenericRepository.Base_Entity;
using GenericRepository.Generic;

namespace GenericRepository.repository
{
    // User-specific repository interface
    public interface IUserRepository : IQueryableRepository<User, Guid>
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUsersByEmailDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetActiveUsersAsync(DateTime since, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);
        Task<PagedResult<User>> GetUsersByCreationDateAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}

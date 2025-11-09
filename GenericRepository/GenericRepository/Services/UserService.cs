using GenericRepository.Base_Entity;
using GenericRepository.Generic;
using GenericRepository.repository;
using GenericRepository.UnitOfWorkPattern;

namespace GenericRepository.Services
{
    // Usage example
    public class UserService(IUnitOfWork unitOfWork)
    {
        private readonly IUserRepository _userRepository = unitOfWork.GetSpecificRepository<IUserRepository>();

        public async Task<User> CreateUserAsync(string username, string email, string? firstName = null, string? lastName = null, CancellationToken cancellationToken = default)
        {
            // Check if username and email are available
            if (!await _userRepository.IsUsernameAvailableAsync(username, cancellationToken))
                throw new InvalidOperationException("Username is already taken.");

            if (!await _userRepository.IsEmailAvailableAsync(email, cancellationToken))
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                Id = User.GenerateNewKey(),
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            return await _userRepository.AddAsync(user, cancellationToken);
        }

        public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetByUsernameAsync(username, cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _userRepository.SearchUsersAsync(searchTerm, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyAsync(string domain, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUsersByEmailDomainAsync(domain, cancellationToken);
        }

        public async Task<PagedResult<User>> GetNewUsersAsync(DateTime since, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var endDate = DateTime.UtcNow;
            return await _userRepository.GetUsersByCreationDateAsync(since, endDate, pageNumber, pageSize, cancellationToken);
        }
    }
}

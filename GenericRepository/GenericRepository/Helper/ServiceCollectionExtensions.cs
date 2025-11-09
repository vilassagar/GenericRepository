using GenericRepository.Data;
using GenericRepository.Generic;
using GenericRepository.UnitOfWorkPattern;

namespace GenericRepository.Helper
{
    // Dependency injection setup
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryPattern(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(EfRepository<,>));
            services.AddScoped(typeof(IQueryableRepository<,>), typeof(EfRepository<,>));

            return services;
        }
    }
}

using GenericRepository.Base_Entity;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Data
{
    // Example DbContext
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }

}

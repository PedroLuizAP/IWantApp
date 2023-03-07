using Flunt.Notifications;
using IWantApp.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IWantApp.Infra.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating (ModelBuilder builder)
        {
            builder.Ignore<Notification>();

            builder.Entity<Product>().Property(p => p.Description).HasMaxLength(255);
            builder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
            builder.Entity<Category>().Property(p => p.Name).HasMaxLength(20).IsRequired();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            configuration.Properties<string>().HaveMaxLength(100);
        }
    }
}

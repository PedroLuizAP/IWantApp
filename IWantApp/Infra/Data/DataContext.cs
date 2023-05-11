using Flunt.Notifications;
using IWantApp.Domain.Orders;
using IWantApp.Domain.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Infra.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<Notification>();

            builder.Entity<Product>().Property(p => p.Description).HasMaxLength(255);
            builder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();
            builder.Entity<Category>().Property(c => c.Name).HasMaxLength(20).IsRequired();
            builder.Entity<Order>().Property(o => o.ClientId).IsRequired();
            builder.Entity<Order>().Property(o => o.DeliveryAddress).IsRequired();
            builder.Entity<Order>().HasMany(o => o.Products).WithMany(p => p.Orders)
                .UsingEntity(x => x.ToTable("OrderProducts"));
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            configuration.Properties<string>().HaveMaxLength(100);
        }
    }
}

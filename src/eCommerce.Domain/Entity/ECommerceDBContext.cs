using eCommerce.Domain.Entity.Order;
using eCommerce.Domain.Entity.Product;
using eCommerce.Domain.Entity.User;
using Microsoft.EntityFrameworkCore;
namespace eCommerce.Domain.Entity;

public class ECommerceDBContext : DbContext
{
    public ECommerceDBContext(DbContextOptions<ECommerceDBContext> options) : base(options)
    {
        
    }
    public DbSet<Users> Users { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema
        modelBuilder.HasDefaultSchema("Entity");

        // Configure each entity to use the "Entity" schema
        modelBuilder.Entity<Users>().ToTable("Users", "Entity");
        modelBuilder.Entity<Products>().ToTable("Products", "Entity");
        modelBuilder.Entity<Orders>().ToTable("Orders", "Entity");
        modelBuilder.Entity<OrderItems>().ToTable("OrderItems", "Entity");
    }

}

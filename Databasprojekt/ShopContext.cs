using Microsoft.EntityFrameworkCore;
using Databasprojekt.Models;

namespace Databasprojekt;

public class ShopContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "databasprojekt.db");
        
        optionsBuilder.UseSqlite(dbPath);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.CategoryId);
            
            e.Property(x => x.CategoryName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(250);
            
            e.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId); 
            
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Price).IsRequired();
            e.Property(x => x.Description).HasMaxLength(250);
            
            
        });
    }
}
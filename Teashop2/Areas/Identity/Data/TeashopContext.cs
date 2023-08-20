using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Teashop2.Models;

namespace Teashop2.Data;

public class TeashopContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderedProduct> OrderedProducts { get; set; }

    public TeashopContext(DbContextOptions<TeashopContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>().ToTable("Products");
        builder.Entity<Category>().ToTable("Categories");
        builder.Entity<Shipment>().ToTable("Shipments");
        builder.Entity<Order>().ToTable("Orders");
        builder.Entity<OrderedProduct>().ToTable("OrderedProducts");
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ApplicationUserIdentityConfiguration());
    }
}

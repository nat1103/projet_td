using Microsoft.EntityFrameworkCore;
using TD1.Models;

namespace TD1.Data;

public class ElectroShopContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<ProductSupplier> ProductSuppliers { get; set; }
    public DbSet<Stock> Stocks { get; set; }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
        {
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=ElectroShop;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=true;");
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;");
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
        {
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=ElectroShop;User Id=SA;Password=P@ssw0rd;TrustServerCertificate=true;");
        }
    }
}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ProductSupplier for Many-to-Many relationship
        modelBuilder.Entity<ProductSupplier>()
            .HasKey(ps => new { ps.ProductId, ps.SupplierId });

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSuppliers)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);

        // Configure OrderLine for One-to-Many relationship with Order and Product
        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.Order)
            .WithMany(o => o.OrderLines)
            .HasForeignKey(ol => ol.OrderId);

        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.Product)
            .WithMany(p => p.OrderLines)
            .HasForeignKey(ol => ol.ProductId);

        // Configure Stock for One-to-One relationship with Product
        modelBuilder.Entity<Stock>()
            .HasKey(s => s.ProductId);

        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Product)
            .WithOne()
            .HasForeignKey<Stock>(s => s.ProductId);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices; // Add this using directive
using TD3.Models;
using TD3.Services;

// Program.cs
class Program
{
    static void Main(string[] args)
    {
        // Determine the database connection string based on the OS
        string connectionString;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;";
        }
        else
        {
            connectionString = "Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";
        }

        // Configuration des services et du DbContext
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ElectroShopContext>(options =>
                options.UseSqlServer(connectionString))
            .AddTransient<ProductService>()
            .AddTransient<ClientService>()
            .BuildServiceProvider();

        var produitService = serviceProvider.GetService<ProductService>();
        var clientService = serviceProvider.GetService<ClientService>();

        clientService.AddClient("Nathan", "3 rue de la paix", "test@test.fr");
        clientService.AddClient("Jean", "5 rue de la liberté", "jean@jean.fr");

        int laptopId = produitService.AddProduct("Laptop", 1000, 10);
        int desktopId = produitService.AddProduct("Desktop", 800, 5);
        Console.WriteLine($"Laptop ID: {laptopId}");
        Console.WriteLine($"Desktop ID: {desktopId}");

        Console.WriteLine("All products:");
        produitService.GetProducts();

        produitService.UpdateProduct(laptopId, "Laptop 2", 2000, 15);

        Console.WriteLine("\nAll products after update:");
        produitService.GetProducts();
    }
}

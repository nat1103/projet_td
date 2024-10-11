using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices; // Add this using directive
using TD3.Models;
using TD3.Services;
using TD3.Services.Seeder;

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
            .AddTransient<ISeederService , ProductSeeder>()
            .AddTransient<ISeederService, ClientSeeder>()
            .AddTransient<ISeederService, OrderSeeder>()
            .AddTransient<DatabaseSeeder>()
            .BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.Seed();
        }

        Console.WriteLine("Données factices insérées.");
    }
}

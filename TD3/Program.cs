using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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
        var serviceProvider = CreateService(connectionString);

        LazyLoadingVsEagerLoading(serviceProvider);


    }

    // Seed the database with fake data
    private static void FakeData(ServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.Seed();
        }

        Console.WriteLine("Données factices insérées.");
    }

    private static ServiceProvider CreateService(String connectionString)
    {
        return new ServiceCollection()
            .AddDbContext<ElectroShopContext>(options =>
                options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseLazyLoadingProxies()
               )
            .AddTransient<ProductService>()
            .AddTransient<ClientService>()
            .AddTransient<ISeederService, ProductSeeder>()
            .AddTransient<ISeederService, ClientSeeder>()
            .AddTransient<ISeederService, OrderSeeder>()
            .AddTransient<DatabaseSeeder>()
            .BuildServiceProvider();
            
    }

    private static void LazyLoadingVsEagerLoading(ServiceProvider serviceProvider)
    {
        var stopwatch = new Stopwatch();
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ElectroShopContext>();

            // Lazy Loading
            stopwatch.Start();
            var order = context.Orders.ToList();
            var numberLazy = 0;
            var number = 0;
            if (order != null)
            {
                foreach (var cmd in order)
                {
                    foreach (var ligne in cmd.OrderLines)
                    {
                        Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Lazy");
                        numberLazy++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Lazy Loading Time: {stopwatch.ElapsedMilliseconds} ms , nombre de requêtes : {numberLazy}");
            Console.WriteLine("-----------------------------------------------------");
            // Eager Loading
            stopwatch.Reset();
            stopwatch.Start();
            var commandes = context.Orders.Include(c => c.OrderLines).ToList();
            foreach (var cmd in commandes)
            {
                foreach (var ligne in cmd.OrderLines)
                {
                    Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Eager");
                    number++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Eager Loading Time: {stopwatch.ElapsedMilliseconds} ms, Nombre de requêtes : {number}");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TD3;
using TD3.Models;
using TD3.Services;
using TD3.Services.Seeder;

class Program
{
    private ServiceProvider serviceProvider;
    private static ServiceProvider InitService() {
        var connectionString = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;";
        }
        else
        {
            connectionString = "Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";
        }
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddElectroShopServices(connectionString);
        return serviceCollection.BuildServiceProvider();
    }


    static void Main(string[] args)
    {
        Choice();
    }

    private static void Choice()
    {
        DisplayConsole displayConsole = new DisplayConsole(InitService().GetRequiredService<ElectroShopContext>());
        String choice = displayConsole.DisplayMenu();
        switch (choice)
        {
            case "1":
                FakeData(InitService());
                break;
            case "2":
                displayConsole.DisplayAllProducts();
                displayConsole.DisplayUpdateProduct();
                break;
            case "3":
                displayConsole.DisplayFailTransaction();
                break;
            case "4":

                displayConsole.LazyLoadingVsEagerLoading();
                break;
            default:
                Console.WriteLine("Choix invalide.");
                Console.WriteLine("Veuillez recommencer.");
                Choice();
                break;
        }
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

    // Comparer le Lazy Loading et l'Eager Loading
    
}

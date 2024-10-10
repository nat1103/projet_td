using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using TD3.Models;
using TD3.Services;

class Program
{
    static void Main(string[] args)
    {
        // Configuration des services et du DbContext
        var serviceProvider = new ServiceCollection()
            .AddDbContext<Context>(options =>
                options.UseSqlServer("Scaffold-DbContext \"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;\" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models"))  // Ajoutez votre chaîne de connexion ici
            .AddTransient<ProductService>()  // Ajout du service pour les produits
            .AddTransient<ClientService>()   // Ajout du service pour les clients
            .BuildServiceProvider();

        // Obtention des services
        var produitService = serviceProvider.GetService<ProductService>();
        var clientService = serviceProvider.GetService<ClientService>();


        // Ajouter des clients
        clientService.AddClient("Nathan", "3 rue de la paix", "test@test.fr");
        clientService.AddClient("Jean", "5 rue de la liberté", "jean@jean.fr");
    }
}

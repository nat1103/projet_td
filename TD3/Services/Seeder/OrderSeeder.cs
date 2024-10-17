using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;

namespace TD3.Services.Seeder
{
    internal class OrderSeeder : ISeederService
    {
        private readonly ElectroShopContext _context;
        private readonly Faker<Order> _orderFaker;
        private readonly Faker<OrderLine> _orderLineFaker;

        public int Order => 3;
        public OrderSeeder(ElectroShopContext context)
        {
            _context = context;

            var clients = _context.Clients.ToList();
            var products = _context.Products.ToList();

            _orderFaker = new Faker<Order>()
                .RuleFor(c => c.Date, f => f.Date.Past(1))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "En traitement", "Livrée", "Annulée" }))
                .RuleFor(c => c.ClientId, f => f.PickRandom(clients).ClientId)
                .RuleFor(c => c.OrderLines, f => new List<OrderLine>());

            _orderLineFaker = new Faker<OrderLine>()
                .RuleFor(l => l.ProductId, f => f.PickRandom(products).ProductId)
                .RuleFor(l => l.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(l => l.UnitPrice, f => f.PickRandom(products).Price);
        }

        public void SeedData()
        {
            Console.WriteLine("Seeding Orders");
            if (!_context.Orders.Any())
            {
                var commandes = new List<Order>();

                for (int i = 0; i < 100; i++)
                {
                    var commande = _orderFaker.Generate();
                    var numberOfLignes = new Random().Next(1, 5);

                    for (int j = 0; j < numberOfLignes; j++)
                    {
                        var ligneCommande = _orderLineFaker.Generate();
                        commande.OrderLines.Add(ligneCommande);
                    }

                    commandes.Add(commande);
                }

                _context.Orders.AddRange(commandes);
                _context.SaveChanges();
            }
        }
    }
}

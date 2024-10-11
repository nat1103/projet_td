// Tests/ApplicationDbContextTests.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TD3.Models;
using TD3.Services;

namespace TD3.Tests
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly ElectroShopContext _context;

        public ApplicationDbContextTests(ElectroShopContext context)
        {

            _context = context;

            // Optionnel : nettoyer la base de données avant chaque test
            _context.Database.EnsureDeleted(); // Supprime la base de données si elle existe déjà
            _context.Database.EnsureCreated(); // Crée la base de données avec le schéma correct

            SeedData(); // Remplit la base de données avec des données de test
        }

        // Nettoyage après les tests
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Supprime la base de données après les tests
            _context.Dispose();
        }

        // Seed data for testing
        private void SeedData()
        {
            var commandes = new List<Order>
            {
                new Order
                {
                    Date = DateTime.Now,
                    Status = "En cours",
                    ClientId = 1,
                },
                new Order
                {
                    Date = DateTime.Now,
                    Status = "En cours",
                    ClientId = 2,
                }
            };

            _context.Orders.AddRange(commandes);
            _context.SaveChanges();
        }

        // Test for Lazy Loading
        [Fact]
        public void TestLazyLoading()
        {
            var commande = _context.Orders.First();

            // Lazy load LignesCommande
            Assert.NotNull(commande.OrderLines); // Les entités liées doivent être chargées de façon lazy
            Assert.Equal(2, commande.OrderLines.Count); // Vérifier le nombre de lignes de commande
        }

        // Test for Eager Loading
        [Fact]
        public void TestEagerLoading()
        {
            var commandes = _context.Orders
                .Include(c => c.OrderLines)
                .ToList();

            Assert.NotEmpty(commandes);
            Assert.Equal(2, commandes[0].OrderLines.Count); // Vérifier le chargement eager
        }

        // Test performance for Lazy vs Eager Loading
        [Fact]
        public void TestLazyVsEagerLoadingPerformance()
        {
            var lazyCommandes = _context.Orders.ToList();
            foreach (var commande in lazyCommandes)
            {
                var lignes = commande.OrderLines.ToList(); // Lazy load each
            }

            var eagerCommandes = _context.Orders
                .Include(c => c.OrderLines)
                .ToList(); // Eager load all
        }
    }
}

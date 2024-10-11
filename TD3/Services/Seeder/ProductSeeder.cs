using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;

namespace TD3.Services.Seeder
{
    internal class ProductSeeder : ISeederService
    {
        private readonly ElectroShopContext _context;
        private readonly Faker<Product> _productFaker;

        public ProductSeeder(ElectroShopContext context)
        {
            _context = context;

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 100));
        }

        public void SeedData()
        {
            if (!_context.Products.Any())
            {
                var produits = _productFaker.Generate(50);
                _context.Products.AddRange(produits);
                _context.SaveChanges();
            }
        }
    }
}

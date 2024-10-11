// ProductService.cs
using Microsoft.EntityFrameworkCore;
using TD3.Models;
using System.Collections.Generic;
using System.Linq;

namespace TD3.Services
{
    
    internal class ProductService
    {
        private readonly Context _context;

        public ProductService(Context context)
        {
            _context = context;
        }

        public Product AddProduct(string name, decimal price, int stock)
        {
            var product = new Product
            {
                Name = name,
                Price = price,
                Stock = stock,
            };

            _context.Add(product);
            _context.SaveChanges();
            return product; // Return the new product's ID
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            _context.AddRange(products);
            _context.SaveChanges();
        }

        public void UpdateProduct(int productId, string name, decimal price, int stock)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Name = name;
                product.Price = price;
                product.Stock = stock;

                // Mark the entity as modified
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Product not found");
            }
        }

        public void GetProducts()
        {
            var products = _context.Products.ToList();
            foreach (var product in products)
            {
                Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.Name}, Price: {product.Price}, Stock: {product.Stock}");
            }
        }
    }
}

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

        public void AddProduct(string name, decimal price, int stock)
        {
            var product = new Product
            {
                Name = name,
                Price = price,
                Stock = stock,
            };

            _context.Add(product);
            _context.SaveChanges();
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            _context.AddRange(products);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product updatedProduct)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
            if (product != null)
            {
                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Stock = updatedProduct.Stock;
        
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

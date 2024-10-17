// ProductService.cs
using Microsoft.EntityFrameworkCore;
using TD3.Models;
using System.Collections.Generic;
using System.Linq;

namespace TD3.Services
{
    
    internal class ProductService
    {
        private readonly ElectroShopContext _electroShopContext;

        public ProductService(ElectroShopContext electroShopContext)
        {
            _electroShopContext = electroShopContext;
        }

        public Product AddProduct(string name, decimal price, int stock)
        {
            var product = new Product
            {
                Name = name,
                Price = price,
                Stock = stock,
            };

            _electroShopContext.Add(product);
            _electroShopContext.SaveChanges();
            return product; // Return the new product's ID
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            _electroShopContext.AddRange(products);
            _electroShopContext.SaveChanges();
        }

        public void UpdateProduct(int productId, string name, decimal price, int stock)
        {
            var product = _electroShopContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Name = name;
                product.Price = price;
                product.Stock = stock;

                // Mark the entity as modified
                _electroShopContext.Entry(product).State = EntityState.Modified;
                _electroShopContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Product not found");
            }
        }

        public List<Product> GetProducts()
        {
            return _electroShopContext.Products.ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;

namespace TD3.Services
{
    internal class ProductService
    {
        private readonly Context _context;

        public ProductService(Context context)
        {
            _context = context;
        }

        public void AddProduct(string name, decimal price , int stock)
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
    }
}

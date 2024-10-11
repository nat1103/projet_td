using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TD3.Models;

namespace TD3.Services
{
    internal class TransactionService
    {
        private readonly Context _context;
        private readonly ProductService _productService;

        public TransactionService(Context context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public void ProcessOrder(Order order)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Save order and order lines
                    _context.Orders.Add(order);
                    _context.SaveChanges();
        
                    foreach (var orderLine in order.OrderLines)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.ProductId == orderLine.ProductId);
                        if (product == null || product.Stock <= 0)
                        {
                            throw new InvalidOperationException($"Product with ID: {orderLine.ProductId} is not available in stock.");
                        }
        
                        _productService.UpdateProduct(product.ProductId, product.Name, product.Price, product.Stock - orderLine.Quantity);
                    }
                    
                    order.Status = "Completed";
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    order.Status = "Failed";
                    _context.SaveChanges();
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
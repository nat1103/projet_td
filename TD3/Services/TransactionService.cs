using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TD3.Models;

namespace TD3.Services
{
    internal class TransactionService
    {
        private readonly ElectroShopContext _electroShopContext;
        private readonly ProductService _productService;

        public TransactionService(ElectroShopContext electroShopContext, ProductService productService)
        {
            _electroShopContext = electroShopContext;
            _productService = productService;
        }

        public void ProcessOrder(Order order)
        {
            using (var transaction = _electroShopContext.Database.BeginTransaction())
            {
                try
                {
                    // Save order and order lines
                    _electroShopContext.Orders.Add(order);
                    _electroShopContext.SaveChanges();
        
                    foreach (var orderLine in order.OrderLines)
                    {
                        var product = _electroShopContext.Products.FirstOrDefault(p => p.ProductId == orderLine.ProductId);
                        if (product == null || product.Stock <= 0)
                        {
                            throw new InvalidOperationException($"Product with ID: {orderLine.ProductId} is not available in stock.");
                        }
        
                        _productService.UpdateProduct(product.ProductId, product.Name, product.Price, product.Stock - orderLine.Quantity);
                    }
                    
                    order.Status = "Completed";
                    _electroShopContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // If an exception occurs, rollback the transaction and display a message
                    Console.WriteLine("Transaction failed. Rolling back...");
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace TD3.Models;



public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Stock? StockNavigation { get; set; }

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}

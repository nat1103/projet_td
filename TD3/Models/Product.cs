using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TD3.Models;


public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0, int.MaxValue)] // Price cannot be negative
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)] // Stock cannot be negative
    public int Stock { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Stock? StockNavigation { get; set; }

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}

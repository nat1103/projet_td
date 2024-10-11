using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TD3.Models;

public partial class OrderLine
{
    [Key]
    public int OrderLineId { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)] // Positive quantity
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, int.MaxValue)] // Price cannot be negative
    public decimal UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TD3.Models;

public partial class Stock
{
    [Key]
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    [Required]
    [Range(0, int.MaxValue)] // Quantity cannot be negative
    public int QuantityInStock { get; set; }

    public virtual Product Product { get; set; } = null!;
}

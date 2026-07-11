using System;
using System.Collections.Generic;

namespace Warehouse.Domain.Entities;
public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public int QuantityInStock { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public Guid SupplierId { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual Supplier Supplier { get; set; } = null!;
}
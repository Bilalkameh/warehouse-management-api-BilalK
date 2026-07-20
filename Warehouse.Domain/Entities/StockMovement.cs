using Warehouse.Domain.Exceptions;

namespace Warehouse.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int QuantityChange { get; private set; }
    public string Reason { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private StockMovement()
    {
    }

    public StockMovement(Product product, int quantityChange, string reason)
    {
        if (quantityChange == 0)
            throw new BusinessRuleException("Quantity change cannot be zero.");

        if (quantityChange < 0 && string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleException("Reason is required when reducing stock.");
        }

        Id = Guid.NewGuid();
        Product = product;
        ProductId = product.Id;
        QuantityChange = quantityChange;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}
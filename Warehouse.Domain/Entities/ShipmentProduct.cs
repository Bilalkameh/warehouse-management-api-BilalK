using Warehouse.Domain.Exceptions;

namespace Warehouse.Domain.Entities;

public class ShipmentProduct
{
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public Shipment Shipment { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }

    internal ShipmentProduct(Shipment shipment, Product product, int quantity)
    {
        if (shipment == null)
            throw new BusinessRuleException(nameof(shipment));

        if (product == null)
            throw new BusinessRuleException(nameof(product));

        if (quantity <= 0)
        {
            throw new BusinessRuleException("Shipment product quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ShipmentId = shipment.Id;
        Shipment = shipment;
        ProductId = product.Id;
        Product = product;
        Quantity = quantity;
    }

    private ShipmentProduct()
    {
    }
}
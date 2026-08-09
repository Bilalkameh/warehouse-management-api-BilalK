using Warehouse.Domain.Enums;
using Warehouse.Domain.Exceptions;

namespace Warehouse.Domain.Entities;

public class Shipment
{
    public Guid Id { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public Supplier Supplier { get; private set; } = null!;
    public string DestinationAddress { get; private set; } = string.Empty;
    public DateTime EstimatedDeliveryDate { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    public ICollection<ShipmentProduct> Products { get; private set; } = new List<ShipmentProduct>();

    public Shipment(string trackingNumber, Supplier supplier, string destinationAddress, DateTime estimatedDeliveryDate)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new BusinessRuleException("Tracking number is required.");

        if (supplier == null)
            throw new BusinessRuleException(nameof(supplier));

        if (!supplier.IsActive)
        {
            throw new BusinessRuleException("A shipment cannot be created for an inactive supplier.");
        }

        if (string.IsNullOrWhiteSpace(destinationAddress))
        {
            throw new BusinessRuleException("Destination address is required.");
        }

        if (estimatedDeliveryDate.Date <= DateTime.UtcNow.Date)
        {
            throw new BusinessRuleException("Estimated delivery date must be in the future.");
        }

        Id = Guid.NewGuid();
        TrackingNumber = trackingNumber;
        SupplierId = supplier.SupplierId;
        Supplier = supplier;
        DestinationAddress = destinationAddress;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        Status = ShipmentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    private Shipment()
    {
    }

    public void AssignProduct(Product product, int quantity)
    {
        if (Status != ShipmentStatus.Pending)
        {
            throw new BusinessRuleException("Products can only be assigned to pending shipments.");
        }

        if (product == null)
            throw new BusinessRuleException(nameof(product));

        if (product.IsArchived)
        {
            throw new BusinessRuleException("Archived products cannot be assigned to a shipment.");
        }

        if (product.SupplierId != SupplierId)
        {
            throw new BusinessRuleException("The product and shipment must belong to the same supplier.");
        }

        if (quantity <= 0)
        {
            throw new BusinessRuleException("Shipment product quantity must be greater than zero.");
        }

        if (Products.Any(item => item.ProductId == product.Id))
        {
            throw new BusinessRuleException("Product is already assigned to this shipment.");
        }

        Products.Add(new ShipmentProduct(this, product, quantity));
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ShipmentStatus newStatus)
    {
        if (Status == newStatus)
            return;

        var isValidTransition = Status switch
        {
            ShipmentStatus.Pending =>
                newStatus is ShipmentStatus.InTransit
                    or ShipmentStatus.Cancelled,

            ShipmentStatus.InTransit =>
                newStatus is ShipmentStatus.Delivered
                    or ShipmentStatus.Cancelled,

            _ => false
        };

        if (!isValidTransition)
        {
            throw new BusinessRuleException($"Invalid shipment status transition from {Status} to {newStatus}.");
        }

        Status = newStatus;
        LastUpdatedAt = DateTime.UtcNow;

        if (newStatus == ShipmentStatus.Delivered)
            DeliveredAt = DateTime.UtcNow;
    }
}
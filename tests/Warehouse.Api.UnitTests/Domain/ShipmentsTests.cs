using FluentAssertions;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Enums;
using Warehouse.Domain.Exceptions;

namespace Warehouse.Api.UnitTests.Domain;

public class ShipmentTests
{
    [Fact]
    public void Constructor_ValidData_CreatesPendingShipment()
    {
        var supplier = CreateSupplier("sup1");

        var shipment = new Shipment(
            "SHIP-001",
            supplier,
            "Beirut warehouse",
            DateTime.UtcNow.AddDays(5));

        shipment.Id.Should().NotBeEmpty();
        shipment.SupplierId.Should().Be(supplier.SupplierId);
        shipment.Status.Should().Be(ShipmentStatus.Pending);
        shipment.Products.Should().BeEmpty();
        shipment.DeliveredAt.Should().BeNull();
    }

    [Fact]
    public void AssignProduct_SameSupplierAndPendingShipment_AddsProduct()
    {
        var supplier = CreateSupplier("sup1");
        var product = CreateProduct(supplier, "KEY-001");
        var shipment = CreateShipment(supplier);

        shipment.AssignProduct(product, 3);

        shipment.Products.Should().ContainSingle(item =>
            item.ProductId == product.Id &&
            item.Quantity == 3);
    }

    [Fact]
    public void AssignProduct_DifferentSupplier_ThrowsBusinessRuleException()
    {
        var shipmentSupplier = CreateSupplier("sup1");
        var productSupplier = CreateSupplier("sup2");
        var product = CreateProduct(productSupplier, "KEY-001");
        var shipment = CreateShipment(shipmentSupplier);

        Action action = () => shipment.AssignProduct(product, 3);

        action.Should()
            .Throw<BusinessRuleException>()
            .WithMessage(
                "The product and shipment must belong to the same supplier.");
    }

    [Fact]
    public void UpdateStatus_InTransitThenDelivered_SetsDeliveredAt()
    {
        var shipment = CreateShipment(CreateSupplier("sup1"));

        shipment.UpdateStatus(ShipmentStatus.InTransit);
        shipment.UpdateStatus(ShipmentStatus.Delivered);

        shipment.Status.Should().Be(ShipmentStatus.Delivered);
        shipment.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStatus_PendingToDelivered_ThrowsBusinessRuleException()
    {
        var shipment = CreateShipment(CreateSupplier("sup1"));

        Action action = () =>
            shipment.UpdateStatus(ShipmentStatus.Delivered);

        action.Should()
            .Throw<BusinessRuleException>()
            .WithMessage(
                "Invalid shipment status transition from Pending to Delivered.");
    }

    private static Shipment CreateShipment(Supplier supplier)
    {
        return new Shipment(
            "SHIP-001",
            supplier,
            "Beirut warehouse",
            DateTime.UtcNow.AddDays(5));
    }

    private static Supplier CreateSupplier(string name)
    {
        return new Supplier(
            name,
            "Lebanon",
            $"{name}@mail.com",
            "+961-1-000000");
    }

    private static Product CreateProduct(
        Supplier supplier,
        string sku)
    {
        return new Product(
            "keyboard",
            sku,
            "gaming keyboard",
            50,
            10,
            supplier,
            DateTime.UtcNow.AddYears(1));
    }
}
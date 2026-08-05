using Warehouse.Domain.Entities;

namespace Warehouse.Api.UnitTests.Builders;

public class ProductBuilder
{
    private string _name = "keyboard2";
    private string _sku = "KEY-002";
    private string _description = "budget keyboard";
    private double _price = 20;
    private int _quantityInStock = 25;

    private Supplier _supplier = new("sup1", "lebanon", "sup1@mail.com", "+12-345-111");

    private DateTime _expiryDate = new(2029, 6, 29, 21, 0, 0, DateTimeKind.Utc);

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithSku(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithPrice(double price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithQuantity(int quantity)
    {
        _quantityInStock = quantity;
        return this;
    }

    public ProductBuilder WithSupplier(Supplier supplier)
    {
        _supplier = supplier;
        return this;
    }
    
    public ProductBuilder WithExpiryDate(DateTime expiryDate)
    {
        _expiryDate = expiryDate;
        return this;
    }

    public Product Build()
    {
        return new Product(_name, _sku, _description, _price, _quantityInStock, _supplier, _expiryDate);
    }
}

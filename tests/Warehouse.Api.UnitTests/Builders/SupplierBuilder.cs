using Warehouse.Domain.Entities;

namespace Warehouse.Api.UnitTests.Builders;

public class SupplierBuilder
{
    private string _name = "sup1";
    private string _country = "lebanon";
    private string _contactEmail = "sup1@mail.com";
    private string _phoneNumber = "+12-345-111";

    public SupplierBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public SupplierBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public SupplierBuilder WithContactEmail(string contactEmail)
    {
        _contactEmail = contactEmail;
        return this;
    }

    public SupplierBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public Supplier Build()
    {
        return new Supplier(_name, _country, _contactEmail, _phoneNumber);
    }
}
# Exercise 1 — Follow-Up AI Response

I missed one required manual edit: the test uses a builder method that does not exist yet.

Open:

tests/Warehouse.Api.UnitTests/Builders/ProductBuilder.cs

Add this method immediately before `Build()`:

public ProductBuilder WithExpiryDate(DateTime expiryDate)
{
    _expiryDate = expiryDate;
    return this;
}


The end of the builder should look like:

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
    return new Product(
        _name,
        _sku,
        _description,
        _price,
        _quantityInStock,
        _supplier,
        _expiryDate);
}


Then rerun:

dotnet test .\tests\Warehouse.Api.UnitTests\Warehouse.Api.UnitTests.csproj --no-restore --filter "FullyQualifiedName~GetExpiringSoonProducts"

The test filter only controls which tests execute; .NET must still compile the entire unit-test project first, 
which is why this missing method prevented the test from running at all.

This is also a useful Exercise 1 evaluation point: the generated instructions referenced a new builder method but initially 
failed to include its implementation. Keep the raw response unchanged and record this later as a human correction.

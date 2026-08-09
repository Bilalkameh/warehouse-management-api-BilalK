Exercise 1  — AI Output Evaluation

The AI-generated implementation did not build initially. It referenced a builder method that it had not included in the required file changes.

The generated unit test used:
new ProductBuilder()
    .WithExpiryDate(startDate.AddDays(10))
    .Build();

However, the existing ProductBuilder did not contain a WithExpiryDate method so this caused a compiler error:

CS1061: 'ProductBuilder' does not contain a definition for'WithExpiryDate'

I had to send send the error and resend the content of the builder file in order to fix it.

The following method was added to: tests/Warehouse.Api.UnitTests/Builders/ProductBuilder.cs

public ProductBuilder WithExpiryDate(DateTime expiryDate)
{
_expiryDate = expiryDate;
return this;
}
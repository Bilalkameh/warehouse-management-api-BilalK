The correct fix is to reject the archived product immediately after retrieving it. This prevents the supplier query, domain mutation, and persistence call.

Important finding: the current `Product.AssignSupplier` method already throws `BusinessRuleException` for archived products. Therefore, the domain currently prevents the actual assignment. The handler defect is its inefficient validation order and lack of an early archived-state check.

### Faulty logic sequence

In the inspected original `AssignSupplierHandler.cs`:

* Line 20 retrieves the product.
* Line 22 retrieves the supplier before validating the product.
* Lines 24–25 check whether the product exists too late.
* No archived-state check occurs before the supplier query.
* Line 30 relies on `Product.AssignSupplier` to reject the archived product.

The problematic block is:

```csharp
var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

if (product == null)
    throw new NotFoundException("Product was not found.");
```

For a missing or archived product, the supplier query is unnecessary.

### 1. Update `AssignSupplierHandler.cs`

Replace the handler with:

```csharp
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.AssignSupplier;

public class AssignSupplierHandler : IRequestHandler<AssignSupplierRequest, AssignSupplierResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<AssignSupplierResponse> Handle(
        AssignSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        if (product.IsArchived)
            throw new BusinessRuleException(
                "Archived products cannot be updated.");

        var supplier = await _supplierRepository.GetByIdAsync(
            request.SupplierId,
            cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier was not found.");

        product.AssignSupplier(supplier);

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new AssignSupplierResponse
        {
            Success = true
        };
    }
}
```

The guard inside `Product.AssignSupplier` should remain. It protects the invariant when the entity is called from anywhere outside this handler.

### 2. Update the unit test

In:

```text
tests/Warehouse.Api.UnitTests/Commands/Products/AssignSupplierHandlerTests.cs
```

Replace the existing archived-product test, or add this test if it does not exist:

```csharp
[Fact]
public async Task Handle_ArchivedProduct_ThrowsBusinessRuleExceptionWithoutUpdatingProduct()
{
    var productRepositoryMock = new Mock<IProductRepository>();
    var supplierRepositoryMock = new Mock<ISupplierRepository>();

    var product = new ProductBuilder().Build();
    var originalSupplier = product.Supplier;
    var originalSupplierId = product.SupplierId;

    product.Archive();

    var request = new AssignSupplierRequest
    {
        ProductId = product.Id,
        SupplierId = Guid.NewGuid()
    };

    productRepositoryMock
        .Setup(repository => repository.GetByIdAsync(
            request.ProductId,
            CancellationToken.None))
        .ReturnsAsync(product);

    var handler = new AssignSupplierHandler(
        productRepositoryMock.Object,
        supplierRepositoryMock.Object);

    Func<Task> action = async () =>
        await handler.Handle(request, CancellationToken.None);

    await action.Should()
        .ThrowAsync<BusinessRuleException>()
        .WithMessage("Archived products cannot be updated.");

    product.Supplier.Should().BeSameAs(originalSupplier);
    product.SupplierId.Should().Be(originalSupplierId);

    supplierRepositoryMock.Verify(
        repository => repository.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
        Times.Never);

    productRepositoryMock.Verify(
        repository => repository.UpdateAsync(
            It.IsAny<Product>(),
            CancellationToken.None),
        Times.Never);
}
```

This proves that:

* The correct exception is thrown.
* The supplier relationship remains unchanged.
* No unnecessary supplier query occurs.
* The archived product is never updated or persisted.

Run:

```powershell
dotnet test .\tests\Warehouse.Api.UnitTests\Warehouse.Api.UnitTests.csproj --no-restore --filter "FullyQualifiedName~Handle_ArchivedProduct_ThrowsBusinessRuleExceptionWithoutUpdatingProduct"

dotnet test .\WarehouseManagement.sln --no-restore
```

Only these files require modification:

```text
Warehouse.Application/Commands/Products/AssignSupplier/AssignSupplierHandler.cs
tests/Warehouse.Api.UnitTests/Commands/Products/AssignSupplierHandlerTests.cs
```

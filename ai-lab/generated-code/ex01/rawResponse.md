Under:
Warehouse.Application/Queries/Products/

Create the directory:
GetExpiringSoonProducts

Inside it, create:
GetExpiringSoonProductsRequest.cs

Paste:
using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetExpiringSoonProducts;

public class GetExpiringSoonProductsRequest : IRequest<List<ProductViewModel>>
{
}



3. Create the query handler

In the same directory, create:

GetExpiringSoonProductsHandler.cs

Paste:

using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetExpiringSoonProducts;

public class GetExpiringSoonProductsHandler
: IRequestHandler<GetExpiringSoonProductsRequest, List<ProductViewModel>>
{
private const int ExpiringSoonDays = 30;

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public GetExpiringSoonProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<List<ProductViewModel>> Handle(
        GetExpiringSoonProductsRequest request,
        CancellationToken cancellationToken)
    {
        var startDate = _timeProvider.GetUtcNow().UtcDateTime.Date;
        var endDateExclusive = startDate.AddDays(ExpiringSoonDays + 1);

        var products = await _productRepository.GetExpiringSoonAsync(
            startDate,
            endDateExclusive,
            cancellationToken);

        return _mapper.Map<List<ProductViewModel>>(products);
    }
}

The calculated interval is:
ExpiryDate >= today
ExpiryDate < today + 31 days



4. Update the repository interface

Open:

Warehouse.Domain/Interfaces/IProductRepository.cs

Add this method before ExistsBySkuAsync:

Task<List<Product>> GetExpiringSoonAsync(
DateTime startDate,
DateTime endDateExclusive,
CancellationToken cancellationToken);

That section should resemble:
Task<List<Product>> GetExpiringProductsAsync(
DateTime date,
CancellationToken cancellationToken);

Task<List<Product>> GetExpiringSoonAsync(
DateTime startDate,
DateTime endDateExclusive,
CancellationToken cancellationToken);

Task<bool> ExistsBySkuAsync(
string sku,
CancellationToken cancellationToken);

Do not replace GetExpiringProductsAsync. It is already used by the background expiry job.



5. Implement the EF Core query

Open:

Warehouse.Infrastructure/Persistence/ProductRepository.cs

Add the following method after GetExpiringProductsAsync and before ExistsBySkuAsync:

public async Task<List<Product>> GetExpiringSoonAsync(
DateTime startDate,
DateTime endDateExclusive,
CancellationToken cancellationToken)
{
return await _context.Products
.Include(product => product.Supplier)
.Where(product =>
product.ExpiryDate >= startDate &&
product.ExpiryDate < endDateExclusive)
.OrderBy(product => product.ExpiryDate)
.ThenBy(product => product.Name)
.ToListAsync(cancellationToken);
}

The filtering is performed by PostgreSQL rather than loading every product into memory.

The supplier is included because ProductViewModel contains SupplierName.



6. Add the controller endpoint

Open:

Warehouse.Presentation/Controllers/ProductsController.cs

Add these two imports:

using Warehouse.Application.Queries.Products.GetExpiringSoonProducts;
using Warehouse.Application.ViewModels;

Inside ProductsController, place the new action after GetAllProducts and before GetProductById:

[HttpGet("expiring-soon")]
[ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetExpiringSoonProducts(
CancellationToken cancellationToken)
{
var products = await _mediator.Send(
new GetExpiringSoonProductsRequest(),
cancellationToken);

    return Ok(products);
}

Combined with the controller-level route:

[Route("api/products")]

this creates:

GET /api/products/expiring-soon

The controller remains thin: it only sends the query and returns HTTP 200 OK.



7. Register TimeProvider

Open:

Warehouse.Presentation/Program.cs

Find:

builder.Services.AddAutoMapper(configuration => { }, typeof(MappingProfile));

Immediately after it, add:

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

The result should be:

builder.Services.AddAutoMapper(configuration => { }, typeof(MappingProfile));
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services.AddScoped<IProductRepository, ProductRepository>();



8. Add the handler unit test

Under:

tests/Warehouse.Api.UnitTests/Queries/Products/

Create:

GetExpiringSoonProductsHandlerTests.cs

Paste:

using AutoMapper;
using FluentAssertions;
using Moq;
using Warehouse.Api.UnitTests.Builders;
using Warehouse.Application.Queries.Products.GetExpiringSoonProducts;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Api.UnitTests.Queries.Products;

public class GetExpiringSoonProductsHandlerTests
{
[Fact]
public async Task Handle_ProductsExpireWithinThirtyDays_ReturnsProducts()
{
var productRepositoryMock = new Mock<IProductRepository>();
var mapperMock = new Mock<IMapper>();
var timeProviderMock = new Mock<TimeProvider>();

        var fixedNow = new DateTimeOffset(
            2026,
            8,
            4,
            15,
            30,
            0,
            TimeSpan.Zero);

        var startDate = fixedNow.UtcDateTime.Date;
        var endDateExclusive = startDate.AddDays(31);

        var products = new List<Product>
        {
            new ProductBuilder()
                .WithExpiryDate(startDate.AddDays(10))
                .Build()
        };

        var expectedResult = new List<ProductViewModel>
        {
            new()
            {
                Id = products[0].Id,
                Name = products[0].Name,
                SKU = products[0].SKU,
                ExpiryDate = products[0].ExpiryDate
            }
        };

        timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(fixedNow);

        productRepositoryMock
            .Setup(repository => repository.GetExpiringSoonAsync(
                startDate,
                endDateExclusive,
                CancellationToken.None))
            .ReturnsAsync(products);

        mapperMock
            .Setup(mapper => mapper.Map<List<ProductViewModel>>(products))
            .Returns(expectedResult);

        var handler = new GetExpiringSoonProductsHandler(
            productRepositoryMock.Object,
            mapperMock.Object,
            timeProviderMock.Object);

        var result = await handler.Handle(
            new GetExpiringSoonProductsRequest(),
            CancellationToken.None);

        result.Should().BeSameAs(expectedResult);

        productRepositoryMock.Verify(
            repository => repository.GetExpiringSoonAsync(
                startDate,
                endDateExclusive,
                CancellationToken.None),
            Times.Once);
    }
}

This test checks that:

The current time is controllable.
The handler calculates the correct date boundaries.
The repository is called once.
The mapped DTO result is returned.


9. Add the controller unit test

Open:

tests/Warehouse.Api.UnitTests/Controllers/ProductsControllerTests.cs

Add these imports:

using Warehouse.Application.Queries.Products.GetExpiringSoonProducts;
using Warehouse.Application.ViewModels;

Inside ProductsControllerTests, add:

[Fact]
public async Task GetExpiringSoonProducts_ReturnsOk()
{
var mediatorMock = new Mock<IMediator>();

    var expectedResult = new List<ProductViewModel>
    {
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Milk",
            SKU = "MILK-001",
            ExpiryDate = new DateTime(2026, 8, 9)
        }
    };

    mediatorMock
        .Setup(mediator => mediator.Send(
            It.IsAny<GetExpiringSoonProductsRequest>(),
            CancellationToken.None))
        .ReturnsAsync(expectedResult);

    var controller = new ProductsController(mediatorMock.Object);

    var result = await controller.GetExpiringSoonProducts(
        CancellationToken.None);

    var okResult = result.Should()
        .BeOfType<OkObjectResult>()
        .Which;

    okResult.Value.Should().BeSameAs(expectedResult);

    mediatorMock.Verify(
        mediator => mediator.Send(
            It.IsAny<GetExpiringSoonProductsRequest>(),
            CancellationToken.None),
        Times.Once);
}

This verifies that the controller:

Sends the correct MediatR request.
Returns OkObjectResult.
Returns the same DTO collection.
Calls MediatR exactly once.
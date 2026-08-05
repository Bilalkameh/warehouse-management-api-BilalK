Supported endpoints
Method	Route	Purpose
POST	/api/shipments	Create a shipment
POST	/api/shipments/{id}/products	Assign a product
GET	/api/shipments/{id}	Track shipment details and status
POST	/api/shipments/{id}/status	Update delivery status

Statuses are:

Pending
InTransit
Delivered
Cancelled

Allowed transitions:

Pending → InTransit
Pending → Cancelled
InTransit → Delivered
InTransit → Cancelled

The status update publishes ShipmentStatusUpdated using shipment.status-updated. The Notification Service consumes it and creates an idempotent supplier-targeted notification. It does not send an email because the existing project has no email-delivery integration.

Folder structure
Layer	Added structure
Domain	Entities/Shipment.cs, ShipmentProduct.cs, Enums/ShipmentStatus.cs, Interfaces/IShipmentRepository.cs
Application	Commands/Shipments, Queries/Shipments, shipment view models and integration event
Infrastructure	ShipmentRepository.cs and EF Core configuration
Presentation	ShipmentsController.cs and dependency registration
Notifications	Event contract, notification handler and RabbitMQ consumer support
Tests	Shipment domain and handler unit tests
Main classes
Warehouse.Domain/Enums/ShipmentStatus.cs
namespace Warehouse.Domain.Enums;

public enum ShipmentStatus
{
Pending,
InTransit,
Delivered,
Cancelled
}
Warehouse.Domain/Entities/Shipment.cs
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

    public ICollection<ShipmentProduct> Products { get; private set; } =
        new List<ShipmentProduct>();

    public Shipment(
        string trackingNumber,
        Supplier supplier,
        string destinationAddress,
        DateTime estimatedDeliveryDate)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new BusinessRuleException(
                "Tracking number is required.");

        if (supplier == null)
            throw new BusinessRuleException(nameof(supplier));

        if (!supplier.IsActive)
        {
            throw new BusinessRuleException(
                "A shipment cannot be created for an inactive supplier.");
        }

        if (string.IsNullOrWhiteSpace(destinationAddress))
        {
            throw new BusinessRuleException(
                "Destination address is required.");
        }

        if (estimatedDeliveryDate.Date <= DateTime.UtcNow.Date)
        {
            throw new BusinessRuleException(
                "Estimated delivery date must be in the future.");
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
            throw new BusinessRuleException(
                "Products can only be assigned to pending shipments.");
        }

        if (product == null)
            throw new BusinessRuleException(nameof(product));

        if (product.IsArchived)
        {
            throw new BusinessRuleException(
                "Archived products cannot be assigned to a shipment.");
        }

        if (product.SupplierId != SupplierId)
        {
            throw new BusinessRuleException(
                "The product and shipment must belong to the same supplier.");
        }

        if (quantity <= 0)
        {
            throw new BusinessRuleException(
                "Shipment product quantity must be greater than zero.");
        }

        if (Products.Any(item => item.ProductId == product.Id))
        {
            throw new BusinessRuleException(
                "Product is already assigned to this shipment.");
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
            throw new BusinessRuleException(
                $"Invalid shipment status transition from {Status} to {newStatus}.");
        }

        Status = newStatus;
        LastUpdatedAt = DateTime.UtcNow;

        if (newStatus == ShipmentStatus.Delivered)
            DeliveredAt = DateTime.UtcNow;
    }
}
Warehouse.Domain/Entities/ShipmentProduct.cs
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

    internal ShipmentProduct(
        Shipment shipment,
        Product product,
        int quantity)
    {
        if (shipment == null)
            throw new BusinessRuleException(nameof(shipment));

        if (product == null)
            throw new BusinessRuleException(nameof(product));

        if (quantity <= 0)
        {
            throw new BusinessRuleException(
                "Shipment product quantity must be greater than zero.");
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
Warehouse.Domain/Interfaces/IShipmentRepository.cs
using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IShipmentRepository
{
Task<Shipment?> GetByIdAsync(
Guid id,
CancellationToken cancellationToken);

    Task<bool> ExistsByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken);

    Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Shipment shipment,
        CancellationToken cancellationToken);
}
Shipment DTOs
// Warehouse.Application/ViewModels/ShipmentProductViewModel.cs

namespace Warehouse.Application.ViewModels;

public class ShipmentProductViewModel
{
public Guid ProductId { get; set; }
public string ProductName { get; set; } = string.Empty;
public string SKU { get; set; } = string.Empty;
public int Quantity { get; set; }
}
// Warehouse.Application/ViewModels/ShipmentViewModel.cs

using Warehouse.Domain.Enums;

namespace Warehouse.Application.ViewModels;

public class ShipmentViewModel
{
public Guid Id { get; set; }
public string TrackingNumber { get; set; } = string.Empty;
public Guid SupplierId { get; set; }
public string SupplierName { get; set; } = string.Empty;
public string DestinationAddress { get; set; } = string.Empty;
public DateTime EstimatedDeliveryDate { get; set; }
public ShipmentStatus Status { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime LastUpdatedAt { get; set; }
public DateTime? DeliveredAt { get; set; }
public List<ShipmentProductViewModel> Products { get; set; } = new();
}
Create shipment
// CreateShipmentRequest.cs

using System.ComponentModel.DataAnnotations;
using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentRequest : IRequest<ShipmentViewModel>
{
[Required(ErrorMessage = "Tracking number is required.")]
[StringLength(100)]
public string TrackingNumber { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Destination address is required.")]
    [StringLength(250)]
    public string DestinationAddress { get; set; } = string.Empty;

    public DateTime EstimatedDeliveryDate { get; set; }
}
// CreateShipmentHandler.cs

using AutoMapper;
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentHandler
: IRequestHandler<CreateShipmentRequest, ShipmentViewModel>
{
private readonly IShipmentRepository _shipmentRepository;
private readonly ISupplierRepository _supplierRepository;
private readonly IMapper _mapper;

    public CreateShipmentHandler(
        IShipmentRepository shipmentRepository,
        ISupplierRepository supplierRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<ShipmentViewModel> Handle(
        CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var trackingNumberExists =
            await _shipmentRepository.ExistsByTrackingNumberAsync(
                request.TrackingNumber,
                cancellationToken);

        if (trackingNumberExists)
        {
            throw new ConflictException(
                "A shipment with this tracking number already exists.");
        }

        var supplier = await _supplierRepository.GetByIdAsync(
                request.SupplierId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Supplier was not found.");

        var shipment = new Shipment(
            request.TrackingNumber,
            supplier,
            request.DestinationAddress,
            request.EstimatedDeliveryDate);

        await _shipmentRepository.AddAsync(
            shipment,
            cancellationToken);

        return _mapper.Map<ShipmentViewModel>(shipment);
    }
}
Assign product
// AssignProductToShipmentRequest.cs

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace Warehouse.Application.Commands.Shipments.AssignProductToShipment;

public class AssignProductToShipmentRequest
: IRequest<AssignProductToShipmentResponse>
{
[JsonIgnore]
public Guid ShipmentId { get; set; }

    public Guid ProductId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Shipment product quantity must be greater than zero.")]
    public int Quantity { get; set; }
}
// AssignProductToShipmentResponse.cs

namespace Warehouse.Application.Commands.Shipments.AssignProductToShipment;

public class AssignProductToShipmentResponse
{
public bool Success { get; set; }
}
// AssignProductToShipmentHandler.cs

using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.AssignProductToShipment;

public class AssignProductToShipmentHandler
: IRequestHandler<
AssignProductToShipmentRequest,
AssignProductToShipmentResponse>
{
private readonly IShipmentRepository _shipmentRepository;
private readonly IProductRepository _productRepository;

    public AssignProductToShipmentHandler(
        IShipmentRepository shipmentRepository,
        IProductRepository productRepository)
    {
        _shipmentRepository = shipmentRepository;
        _productRepository = productRepository;
    }

    public async Task<AssignProductToShipmentResponse> Handle(
        AssignProductToShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
                request.ShipmentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Shipment was not found.");

        var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Product was not found.");

        shipment.AssignProduct(product, request.Quantity);

        await _shipmentRepository.UpdateAsync(
            shipment,
            cancellationToken);

        return new AssignProductToShipmentResponse
        {
            Success = true
        };
    }
}
Get shipment
// GetShipmentByIdRequest.cs

using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Shipments.GetShipmentById;

public class GetShipmentByIdRequest : IRequest<ShipmentViewModel>
{
public Guid ShipmentId { get; set; }
}
// GetShipmentByIdHandler.cs

using AutoMapper;
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Shipments.GetShipmentById;

public class GetShipmentByIdHandler
: IRequestHandler<GetShipmentByIdRequest, ShipmentViewModel>
{
private readonly IShipmentRepository _shipmentRepository;
private readonly IMapper _mapper;

    public GetShipmentByIdHandler(
        IShipmentRepository shipmentRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<ShipmentViewModel> Handle(
        GetShipmentByIdRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
                request.ShipmentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Shipment was not found.");

        return _mapper.Map<ShipmentViewModel>(shipment);
    }
}
Update shipment status and notify supplier
// UpdateShipmentStatusRequest.cs

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;
using Warehouse.Domain.Enums;

namespace Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusRequest
: IRequest<UpdateShipmentStatusResponse>
{
[JsonIgnore]
public Guid ShipmentId { get; set; }

    [Required(ErrorMessage = "Shipment status is required.")]
    public ShipmentStatus? Status { get; set; }
}
// UpdateShipmentStatusResponse.cs

namespace Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusResponse
{
public bool Success { get; set; }
}
// UpdateShipmentStatusHandler.cs

using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusHandler
: IRequestHandler<
UpdateShipmentStatusRequest,
UpdateShipmentStatusResponse>
{
private readonly IShipmentRepository _shipmentRepository;
private readonly IWarehouseEventPublisher _eventPublisher;
private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public UpdateShipmentStatusHandler(
        IShipmentRepository shipmentRepository,
        IWarehouseEventPublisher eventPublisher,
        ICorrelationIdAccessor correlationIdAccessor)
    {
        _shipmentRepository = shipmentRepository;
        _eventPublisher = eventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task<UpdateShipmentStatusResponse> Handle(
        UpdateShipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Status == null)
        {
            throw new BadRequestException(
                "Shipment status is required.");
        }

        var shipment = await _shipmentRepository.GetByIdAsync(
                request.ShipmentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Shipment was not found.");

        if (shipment.Status == request.Status.Value)
        {
            return new UpdateShipmentStatusResponse
            {
                Success = true
            };
        }

        shipment.UpdateStatus(request.Status.Value);

        await _shipmentRepository.UpdateAsync(
            shipment,
            cancellationToken);

        var shipmentStatusUpdated = new ShipmentStatusUpdated
        {
            CorrelationId = _correlationIdAccessor.CorrelationId,
            RelatedEntityId = shipment.Id,
            SupplierId = shipment.SupplierId,
            SupplierName = shipment.Supplier.Name,
            SupplierEmail = shipment.Supplier.ContactEmail,
            TrackingNumber = shipment.TrackingNumber,
            Status = shipment.Status.ToString()
        };

        await _eventPublisher.PublishAsync(
            shipmentStatusUpdated,
            WarehouseEventRoutingKeys.ShipmentStatusUpdated,
            cancellationToken);

        return new UpdateShipmentStatusResponse
        {
            Success = true
        };
    }
}
ShipmentStatusUpdated.cs
namespace Warehouse.Application.IntegrationEvents;

public class ShipmentStatusUpdated : WarehouseEvent
{
public ShipmentStatusUpdated()
{
EventType = nameof(ShipmentStatusUpdated);
RelatedEntityType = "Shipment";
Severity = "Information";
}

    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierEmail { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

Add the routing key to both WarehouseEventRoutingKeys classes:

public const string ShipmentStatusUpdated =
"shipment.status-updated";
Warehouse.Infrastructure/Persistence/ShipmentRepository.cs
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ShipmentRepository : IShipmentRepository
{
private readonly WarehouseDbContext _context;

    public ShipmentRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments
            .Include(shipment => shipment.Supplier)
            .Include(shipment => shipment.Products)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(
                shipment => shipment.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken)
    {
        return await _context.Shipments.AnyAsync(
            shipment =>
                shipment.TrackingNumber == trackingNumber,
            cancellationToken);
    }

    public async Task AddAsync(
        Shipment shipment,
        CancellationToken cancellationToken)
    {
        await _context.Shipments.AddAsync(
            shipment,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Shipment shipment,
        CancellationToken cancellationToken)
    {
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

Add to WarehouseDbContext:

public DbSet<Shipment> Shipments => Set<Shipment>();

public DbSet<ShipmentProduct> ShipmentProducts =>
Set<ShipmentProduct>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Shipment>()
        .HasIndex(shipment => shipment.TrackingNumber)
        .IsUnique();

    modelBuilder.Entity<Shipment>()
        .Property(shipment => shipment.Status)
        .HasConversion<string>();

    modelBuilder.Entity<ShipmentProduct>()
        .HasIndex(item => new
        {
            item.ShipmentId,
            item.ProductId
        })
        .IsUnique();
}

Add to MappingProfile:

CreateMap<ShipmentProduct, ShipmentProductViewModel>()
.ForMember(
destination => destination.ProductName,
options => options.MapFrom(
source => source.Product.Name))
.ForMember(
destination => destination.SKU,
options => options.MapFrom(
source => source.Product.SKU));

CreateMap<Shipment, ShipmentViewModel>()
.ForMember(
destination => destination.SupplierName,
options => options.MapFrom(
source => source.Supplier.Name));
Warehouse.Presentation/Controllers/ShipmentsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Shipments.AssignProductToShipment;
using Warehouse.Application.Commands.Shipments.CreateShipment;
using Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;
using Warehouse.Application.Queries.Shipments.GetShipmentById;
using Warehouse.Presentation.Authorization;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize(Policy = AuthorizationPolicies.User)]
public class ShipmentsController : ControllerBase
{
private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipmentById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetShipmentByIdRequest
        {
            ShipmentId = id
        };

        var shipment = await _mediator.Send(
            request,
            cancellationToken);

        return Ok(shipment);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> CreateShipment(
        [FromBody] CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _mediator.Send(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetShipmentById),
            new { id = shipment.Id },
            shipment);
    }

    [HttpPost("{id}/products")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> AssignProduct(
        [FromRoute] Guid id,
        [FromBody] AssignProductToShipmentRequest request,
        CancellationToken cancellationToken)
    {
        request.ShipmentId = id;

        var result = await _mediator.Send(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id}/status")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateShipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        request.ShipmentId = id;

        var result = await _mediator.Send(
            request,
            cancellationToken);

        return Ok(result);
    }
}

Add this repository registration to Warehouse.Presentation/Program.cs:

builder.Services.AddScoped<
IShipmentRepository,
ShipmentRepository>();

String enum serialization was also added:

using System.Text.Json.Serialization;
builder.Services.AddControllers(options =>
{
options.Filters.AddService<ActionLoggingFilter>();
options.Filters.AddService<ModelValidationFilter>();
})
.AddJsonOptions(options =>
{
options.JsonSerializerOptions.Converters.Add(
new JsonStringEnumConverter());
});
Request examples

Create a shipment:

{
"trackingNumber": "SHIP-001",
"supplierId": "SUPPLIER-GUID",
"destinationAddress": "Beirut warehouse",
"estimatedDeliveryDate": "2026-08-20T10:00:00Z"
}

Assign a product:

{
"productId": "PRODUCT-GUID",
"quantity": 5
}

Update status:

{
"status": "InTransit"
}


Generate the migration and test

Run from the repository root:

dotnet build .\WarehouseManagement.sln --no-restore
dotnet ef migrations add AddShipmentTracking `
  --project .\Warehouse.Infrastructure\Warehouse.Infrastructure.csproj `
--startup-project .\Warehouse.Presentation\Warehouse.Presentation.csproj

Inspect the generated migration, then apply it:

dotnet ef database update `
  --project .\Warehouse.Infrastructure\Warehouse.Infrastructure.csproj `
--startup-project .\Warehouse.Presentation\Warehouse.Presentation.csproj

Run the tests and build both solutions:

dotnet test .\WarehouseManagement.sln --no-restore
dotnet build .\Warehouse.Notifications.sln --no-restore

Focused tests were added here:

ShipmentTests.cs
CreateShipmentHandlerTests.cs
UpdateShipmentStatusHandlerTests.cs

I could not execute the build locally because this runtime does not contain the .NET SDK.
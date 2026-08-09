using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.AssignProductToShipment;

public class AssignProductToShipmentHandler
    : IRequestHandler<AssignProductToShipmentRequest, AssignProductToShipmentResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IProductRepository _productRepository;

    public AssignProductToShipmentHandler(IShipmentRepository shipmentRepository, IProductRepository productRepository)
    {
        _shipmentRepository = shipmentRepository;
        _productRepository = productRepository;
    }

    public async Task<AssignProductToShipmentResponse> Handle(AssignProductToShipmentRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken)
                       ?? throw new NotFoundException("Shipment was not found.");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                      ?? throw new NotFoundException("Product was not found.");

        shipment.AssignProduct(product, request.Quantity);

        await _shipmentRepository.UpdateAsync(shipment, cancellationToken);

        return new AssignProductToShipmentResponse
        {
            Success = true
        };
    }
}
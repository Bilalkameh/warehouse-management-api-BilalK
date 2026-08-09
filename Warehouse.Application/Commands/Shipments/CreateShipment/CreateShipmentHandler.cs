using AutoMapper;
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentHandler : IRequestHandler<CreateShipmentRequest, ShipmentViewModel>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public CreateShipmentHandler(IShipmentRepository shipmentRepository, ISupplierRepository supplierRepository, IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<ShipmentViewModel> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        var trackingNumberExists =
            await _shipmentRepository.ExistsByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

        if (trackingNumberExists)
        {
            throw new ConflictException("A shipment with this tracking number already exists.");
        }

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
                       ?? throw new NotFoundException("Supplier was not found.");

        var shipment = new Shipment(request.TrackingNumber, supplier, request.DestinationAddress, request.EstimatedDeliveryDate);

        await _shipmentRepository.AddAsync(shipment, cancellationToken);

        return _mapper.Map<ShipmentViewModel>(shipment);
    }
}
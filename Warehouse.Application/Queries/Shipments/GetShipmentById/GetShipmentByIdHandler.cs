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

    public GetShipmentByIdHandler(IShipmentRepository shipmentRepository, IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<ShipmentViewModel> Handle(GetShipmentByIdRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken)
                       ?? throw new NotFoundException("Shipment was not found.");

        return _mapper.Map<ShipmentViewModel>(shipment);
    }
}
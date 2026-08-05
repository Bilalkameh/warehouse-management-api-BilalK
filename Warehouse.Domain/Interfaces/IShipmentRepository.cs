using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken);

    Task AddAsync(Shipment shipment, CancellationToken cancellationToken);

    Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken);
}
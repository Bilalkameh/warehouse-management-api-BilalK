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
            .FirstOrDefaultAsync(shipment => shipment.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        return await _context.Shipments.AnyAsync(shipment => shipment.TrackingNumber == trackingNumber, cancellationToken);
    }

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken)
    {
        await _context.Shipments.AddAsync(shipment, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken)
    {
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
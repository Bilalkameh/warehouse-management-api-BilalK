namespace Warehouse.Application.Interfaces;

public interface ICorrelationIdAccessor
{
    string CorrelationId { get; }
}
using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Stock;

public class StockAdjustmentRequest : IRequest<ProductViewModel>
{
    public Guid ProductId { get; set; }
    public int QuantityChange { get; set; }
    public string Reason { get; set; } = string.Empty;
}
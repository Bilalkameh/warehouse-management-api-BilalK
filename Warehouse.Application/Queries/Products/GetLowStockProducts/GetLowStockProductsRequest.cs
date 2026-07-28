using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetLowStockProducts;

public class GetLowStockProductsRequest : IRequest<List<ProductViewModel>>
{
}
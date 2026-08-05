using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetOutOfStockProducts;

public class GetOutOfStockProductsRequest : IRequest<List<ProductViewModel>>
{
}
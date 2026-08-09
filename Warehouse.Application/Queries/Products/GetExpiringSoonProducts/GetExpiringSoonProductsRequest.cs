using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetExpiringSoonProducts;

public class GetExpiringSoonProductsRequest : IRequest<List<ProductViewModel>>
{
}
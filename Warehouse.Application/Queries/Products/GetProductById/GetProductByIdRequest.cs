using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdRequest : IRequest<ProductViewModel>
{
    public Guid ProductId { get; set; }
}
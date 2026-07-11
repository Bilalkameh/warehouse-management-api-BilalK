using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSuppliers;

public class GetSuppliersHandler
    : IRequestHandler<GetSuppliersRequest, List<SupplierViewModel>>
{
    private readonly ISupplierRepository _repository;
    private readonly IMapper _mapper;

    public GetSuppliersHandler(
        ISupplierRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<List<SupplierViewModel>> Handle(
        GetSuppliersRequest request,
        CancellationToken cancellationToken)
    {
        var suppliers = _repository.GetAll();

        var viewModels =
            _mapper.Map<List<SupplierViewModel>>(suppliers);

        return Task.FromResult(viewModels);
    }
}
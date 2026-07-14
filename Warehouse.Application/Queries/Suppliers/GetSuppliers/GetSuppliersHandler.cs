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

    public async Task<List<SupplierViewModel>> Handle(GetSuppliersRequest request, CancellationToken cancellationToken)
    {
        var suppliers = await _repository.GetAllAsync(cancellationToken);
        
          return _mapper.Map<List<SupplierViewModel>>(suppliers);
    }
}
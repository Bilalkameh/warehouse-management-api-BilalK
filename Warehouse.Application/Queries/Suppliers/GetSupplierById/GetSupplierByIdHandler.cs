using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierById;

public class GetSupplierByIdHandler
    : IRequestHandler<GetSupplierByIdRequest, SupplierViewModel?>
{
    private readonly ISupplierRepository _repository;
    private readonly IMapper _mapper;

    public GetSupplierByIdHandler(
        ISupplierRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SupplierViewModel?> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
            return null;

        return _mapper.Map<SupplierViewModel>(supplier);
    }
}
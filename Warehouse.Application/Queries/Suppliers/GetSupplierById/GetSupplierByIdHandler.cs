using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;

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
            throw new NotFoundException("Product was not found.");

        return _mapper.Map<SupplierViewModel>(supplier);
    }
}
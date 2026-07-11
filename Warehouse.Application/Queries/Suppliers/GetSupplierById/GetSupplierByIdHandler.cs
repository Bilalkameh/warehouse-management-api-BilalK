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

    public Task<SupplierViewModel?> Handle(
        GetSupplierByIdRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = _repository.GetById(request.SupplierId);

        if (supplier == null)
            return Task.FromResult<SupplierViewModel?>(null);

        var viewModel =
            _mapper.Map<SupplierViewModel>(supplier);

        return Task.FromResult<SupplierViewModel?>(viewModel);
    }
}
using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.CreateSupplier;

public class CreateSupplierHandler
    : IRequestHandler<CreateSupplierRequest, SupplierViewModel>
{
    private readonly ISupplierRepository _repository;
    private readonly IMapper _mapper;

    public CreateSupplierHandler(
        ISupplierRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<SupplierViewModel> Handle(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = new Supplier(
            request.Name,
            request.Country,
            request.ContactEmail,
            request.PhoneNumber);

        _repository.Add(supplier);
        var viewModel =
            _mapper.Map<SupplierViewModel>(supplier);

        return Task.FromResult(viewModel);
    }
}
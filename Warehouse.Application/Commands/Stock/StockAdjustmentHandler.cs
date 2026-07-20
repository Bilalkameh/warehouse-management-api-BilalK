using AutoMapper;
using FluentValidation;
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Stock;

public class StockAdjustmentHandler : IRequestHandler<StockAdjustmentRequest, ProductViewModel>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<StockAdjustmentRequest> _validator;
    private readonly IMapper _mapper;

    public StockAdjustmentHandler(IProductRepository productRepository, IValidator<StockAdjustmentRequest> validator, IMapper mapper)
    {
        _productRepository = productRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ProductViewModel> Handle(StockAdjustmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        var newQuantity = product.QuantityInStock + request.QuantityChange;

        product.UpdateQuantity(newQuantity);

        var movement = new StockMovement(product, request.QuantityChange, request.Reason);

        await _productRepository.AdjustStockAsync(product, movement, cancellationToken);
        
            
        return _mapper.Map<ProductViewModel>(product);
    }
}
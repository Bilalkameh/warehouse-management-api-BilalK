using FluentValidation;

namespace Warehouse.Application.Commands.Stock;

// Here we used fluent validation 
public class StockAdjustmentRequestValidator : AbstractValidator<StockAdjustmentRequest>
{
    public StockAdjustmentRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty()
            .WithMessage("Product id is required.");

        RuleFor(request => request.QuantityChange)
            .NotEqual(0)
            .WithMessage("Quantity change cannot be zero.");

        RuleFor(request => request.Reason)
            .MaximumLength(200)
            .WithMessage("Reason cannot exceed 200 characters.");

        RuleFor(request => request.Reason)
            .NotEmpty()
            .When(request => request.QuantityChange < 0)
            .WithMessage("Reason is required when reducing stock.");
    }
}
using FluentValidation;

namespace Application.Requests.Bicycles.Commands.UpdateBicycle;

public class UpdateBicycleCommandValidator : AbstractValidator<UpdateBicycleCommand>
{
    public UpdateBicycleCommandValidator()
    {
        RuleFor(x => x.ManufactureDate).GreaterThanOrEqualTo(new DateTime(1900, 1, 1));
    }
}
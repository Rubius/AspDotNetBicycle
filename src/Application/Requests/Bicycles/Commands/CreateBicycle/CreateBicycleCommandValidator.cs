using FluentValidation;

namespace Application.Requests.Bicycles.Commands.CreateBicycle;

public class CreateBicycleCommandValidator : AbstractValidator<CreateBicycleCommand>
{
    public CreateBicycleCommandValidator()
    {
        RuleFor(x => x.ManufactureDate).GreaterThanOrEqualTo(new DateTime(1900, 1, 1));
    }
}
using FluentValidation;

namespace Application.Requests.BicycleModels.Commands.CreateBicycleModel;

public class CreateBicycleModelCommandValidator : AbstractValidator<CreateBicycleModelCommand>
{
    public CreateBicycleModelCommandValidator()
    {
        RuleFor(x => x.LifeTimeYears).GreaterThan(0);
    }
}
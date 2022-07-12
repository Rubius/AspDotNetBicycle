using FluentValidation;

namespace Application.Requests.BicycleModels.Commands.UpdateBicycleModel;

public class UpdateBicycleModelCommandValidator : AbstractValidator<UpdateBicycleModelCommand>
{
    public UpdateBicycleModelCommandValidator()
    {
        RuleFor(x => x.LifeTimeYears).GreaterThan(0);
    }
}
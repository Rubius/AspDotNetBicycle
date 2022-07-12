using FluentValidation;

namespace Application.Requests.BicycleBrands.Commands.UpdateBicycleBrand;

public class UpdateBicycleBrandCommandValidator : AbstractValidator<UpdateBicycleBrandCommand>
{
    public UpdateBicycleBrandCommandValidator()
    {
        RuleFor(x => x.LifeTimeYears).GreaterThan(0);
    }
}
using FluentValidation;

namespace Application.Requests.BicycleBrands.Commands.CreateBicycleBrand;

public class CreateBicycleBrandCommandValidator : AbstractValidator<CreateBicycleBrandCommand>
{
    public CreateBicycleBrandCommandValidator()
    {
        RuleFor(x => x.LifeTimeYears).GreaterThan(0);
    }
}
using FluentValidation;

namespace Application.Requests.Rides.Commands.FinishRide;

public class FinishRideCommandValidator : AbstractValidator<FinishRideCommand>
{
    public FinishRideCommandValidator()
    {
        RuleFor(x => x.Distance).GreaterThan(0);
    }
}
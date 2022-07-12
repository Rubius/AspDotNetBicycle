using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.Enums;
using MediatR;

namespace Application.Requests.Service.Command;

public class SeedDbCommand : IRequest
{
}

public class SeedDbCommandHandler : IRequestHandler<SeedDbCommand>
{
    private readonly IApplicationDbContext _context;

    public SeedDbCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SeedDbCommand request, CancellationToken cancellationToken)
    {
        var russianBicycleModel = new BicycleModel("AB-1569", BicycleModelClass.A, new Address("Russia", "Tomsk", "Tomsk", "Kievskaya"))
        {
            LifeTimeYears = 5
        };
        var germanBicycleModel = new BicycleModel("Langustin", BicycleModelClass.C, new Address("Germany", "Bavaria"))
        {
            LifeTimeYears = 10
        };

        var createRentalAddress = () => new Address("Russia", "Saint-Petersburg");
        var bicycles = new List<Bicycle>{
            new(createRentalAddress())
            {
                IsWrittenOff = false,
                ManufactureDate = new DateTime(2020, 10, 22),
                Model = russianBicycleModel
            },
            new(createRentalAddress())
            {
                IsWrittenOff = false,
                ManufactureDate = new DateTime(2018, 10, 22),
                Model = germanBicycleModel
            },
            new(createRentalAddress())
            {
                IsWrittenOff = true,
                ManufactureDate = new DateTime(2016, 10, 22),
                Model = russianBicycleModel
            },
            new(createRentalAddress())
            {
                IsWrittenOff = false,
                ManufactureDate = new DateTime(2019, 10, 22),
                Model = germanBicycleModel
            },
        };

        await _context.Bicycles.AddRangeAsync(bicycles, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
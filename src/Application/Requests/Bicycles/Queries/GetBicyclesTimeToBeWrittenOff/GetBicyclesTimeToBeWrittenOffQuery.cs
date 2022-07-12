using Application.Requests.Bicycles.Queries.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.GetBicyclesTimeToBeWrittenOff;
public class GetBicyclesTimeToBeWrittenOffQuery : IRequest<IEnumerable<BicycleTimeToBeWrittenOffDto>>
{
    [JsonProperty(Required = Required.Always)]
    public string RentalPointCity { get; set; } = string.Empty;
}

public class GetBicyclesTimeToBeWrittenOffHandler : IRequestHandler<GetBicyclesTimeToBeWrittenOffQuery, IEnumerable<BicycleTimeToBeWrittenOffDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBicyclesTimeToBeWrittenOffHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BicycleTimeToBeWrittenOffDto>> Handle(GetBicyclesTimeToBeWrittenOffQuery request, CancellationToken cancellationToken)
    {
        var bicyclesByCityQuery = Bicycle.FilterByCity(_context.Bicycles, request.RentalPointCity);
        var bicyclesTimeToBeWritten = await bicyclesByCityQuery
            .Include(x => x.Model)
            .Select(x => new BicycleTimeToBeWrittenOffDto
            {
                Id = x.Id,
                TimeToBeWrittenOff = x.CalculateTimeToWriteOff(DateTime.Now)
            }).ToListAsync(cancellationToken);

        return bicyclesTimeToBeWritten;
    }
}

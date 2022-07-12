using Application.Common.Exceptions;
using Application.Requests.Bicycles.Queries.Dto;
using AutoMapper;
using Domain.Entities;
using Localization.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.GetBicyclesWillBeWrittenOffThisYear;

public class GetBicyclesWillBeWrittenOffThisYearQuery : IRequest<IEnumerable<ShortBicycleDto>>
{
    [JsonProperty(Required = Required.Always)]
    public ulong ModelId { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    public string? RentalPointCity { get; set; }
}

public class GetBicyclesWillBeWrittenOffThisYearHandler : IRequestHandler<GetBicyclesWillBeWrittenOffThisYearQuery, IEnumerable<ShortBicycleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBicyclesWillBeWrittenOffThisYearHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShortBicycleDto>> Handle(GetBicyclesWillBeWrittenOffThisYearQuery request, CancellationToken cancellationToken)
    {
        var model = await _context.BicycleModels.Where(x => x.Id == request.ModelId).FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            throw new BadRequestException(Resources.EntityNotExists, new { ModelsId = request.ModelId });
        }

        var bicyclesInRequestedCityQuery = Bicycle.FilterByCity(_context.Bicycles, request.RentalPointCity);
        var bicyclesBeWrittenOffThisYear = await model.GetBicyclesWillBeWrittenOffThisYear(bicyclesInRequestedCityQuery)
            .ToListAsync(cancellationToken);

        return bicyclesBeWrittenOffThisYear.Select(x => _mapper.Map<ShortBicycleDto>(x));
    }
}

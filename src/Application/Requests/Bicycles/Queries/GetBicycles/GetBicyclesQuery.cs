using Application.Common.Dto;
using Application.Requests.Bicycles.Queries.Dto;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.EntitiesExtensions;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.GetBicycles;

public class GetBicyclesQuery : IRequest<IList<ShortBicycleDto>>
{
    [JsonProperty(Required = Required.Default)]
    public ulong? BicycleModelId { get; set; }

    [JsonProperty(Required = Required.Default)]
    public AddressDto? AddressDto { get; set; }

    [JsonProperty(Required = Required.Default)]
    public BicycleTechnicalStatus? TechnicalStatus { get; set; }

    [JsonProperty(Required = Required.Default)]
    public bool? NeededTechnicalInspection { get; set; }
}

public class GetAllBicyclesQueryHandler : IRequestHandler<GetBicyclesQuery, IList<ShortBicycleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBicyclesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<ShortBicycleDto>> Handle(GetBicyclesQuery request, CancellationToken cancellationToken)
    {
        var address = _mapper.Map<Address>(request.AddressDto);

        var query = _context.Bicycles.AsQueryable();

        if (request.BicycleModelId is not null)
        {
            query = query.Where(x => x.ModelId == request.BicycleModelId);
        }
        if (request.TechnicalStatus is not null)
        {
            query = query.Where(x => x.TechnicalStatus == request.TechnicalStatus);
        }
        if (request.NeededTechnicalInspection is not null)
        {
            query = query.Where(x => x.NeedTechnicalInspection == request.NeededTechnicalInspection);
        }

        var result = await query
        .ByAddress(address)
        .Select(x => _mapper.Map<Bicycle, ShortBicycleDto>(x))
        .ToListAsync(cancellationToken);

        return result;
    }
}
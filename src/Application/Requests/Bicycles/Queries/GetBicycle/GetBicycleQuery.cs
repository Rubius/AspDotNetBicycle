using Application.Common.Exceptions;
using Application.Requests.Bicycles.Queries.Dto;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.GetBicycle;

public class GetBicycleQuery : IRequest<FullBicycleDto>
{
    public GetBicycleQuery(ulong id)
    {
        Id = id;
    }

    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; set; }
}

public class GetBicycleCommandHandler : IRequestHandler<GetBicycleQuery, FullBicycleDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBicycleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FullBicycleDto> Handle(GetBicycleQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Bicycles
            .Include(x => x.Model)
            .Where(x => x.Id == request.Id)
            .Select(x => _mapper.Map<Bicycle, FullBicycleDto>(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Bicycle), request.Id.ToString());
        }

        return entity;
    }
}
using Application.Common.Exceptions;
using Application.Requests.BicycleModels.Queries.Dto;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.BicycleModels.Queries.GetBicycleModel;

public class GetBicycleModelQuery : IRequest<BicycleModelDto>
{
    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; }

    public GetBicycleModelQuery(ulong id)
    {
        Id = id;
    }

    public class GetBicycleModelQueryHandler : IRequestHandler<GetBicycleModelQuery, BicycleModelDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetBicycleModelQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BicycleModelDto> Handle(GetBicycleModelQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.BicycleModels
                .Where(x => x.Id == request.Id)
                .Select(x => _mapper.Map<BicycleModelDto>(x))
                .FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(BicycleModel), request.Id.ToString());
            }

            return entity;
        }
    }
}
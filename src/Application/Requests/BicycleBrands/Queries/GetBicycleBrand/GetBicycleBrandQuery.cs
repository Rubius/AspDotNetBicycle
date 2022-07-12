using Application.Common.Exceptions;
using Application.Requests.BicycleBrands.Queries.Dto;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.BicycleBrands.Queries.GetBicycleBrand;

public class GetBicycleBrandQuery : IRequest<BicycleBrandDto>
{
    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; }

    public GetBicycleBrandQuery(ulong id)
    {
        Id = id;
    }

    public class GetBicycleBrandQueryHandler : IRequestHandler<GetBicycleBrandQuery, BicycleBrandDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetBicycleBrandQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BicycleBrandDto> Handle(GetBicycleBrandQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.BicycleBrands
                .Where(x => x.Id == request.Id)
                .Select(x => _mapper.Map<BicycleBrandDto>(x))
                .FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(BicycleBrand), request.Id.ToString());
            }

            return entity;
        }
    }
}
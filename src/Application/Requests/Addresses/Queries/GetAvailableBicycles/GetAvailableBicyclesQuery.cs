using Application.Common.Dto;
using Application.Requests.Addresses.Queries.GetAvailableBicycles.Dto;
using AutoMapper;
using Domain.Entities.ValueObjects;
using Domain.EntitiesExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Addresses.Queries.GetAvailableBicycles
{
    public class GetAvailableBicyclesQuery : IRequest<IEnumerable<BicycleBrandCountDto>>
    {
        [JsonProperty(Required = Required.Always)]
        public AddressDto AddressDto { get; set; } = new AddressDto();
    }

    public class GetAvailableBicyclesQueryHandler : IRequestHandler<GetAvailableBicyclesQuery, IEnumerable<BicycleBrandCountDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAvailableBicyclesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BicycleBrandCountDto>> Handle(GetAvailableBicyclesQuery request, CancellationToken cancellationToken)
        {
            var address = _mapper.Map<Address>(request.AddressDto);

            var bicyclesModelsCounts = await _context.Bicycles.AsQueryable()
                .OnlyAvailableForRent(_context.Rides.AsQueryable())
                .ByAddress(address)
                .GroupBy(x => new { x.BrandId, x.Brand!.Name })
                .Select(x => new BicycleBrandCountDto 
                {
                    Id = x.Key.BrandId,
                    Name = x.Key.Name,
                    Count = x.Count()
                }).ToListAsync(cancellationToken);

            return bicyclesModelsCounts;
        }
    }
}

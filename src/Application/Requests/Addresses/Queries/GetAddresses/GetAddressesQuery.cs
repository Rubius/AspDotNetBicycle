using Application.Common.Dto;
using Application.Requests.Addresses.Queries.GetAddresses.Models;
using AutoMapper;
using Domain.Entities.ValueObjects;
using Domain.EntitiesExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Addresses.Queries.GetAddresses
{
    public class GetAddressesQuery : IRequest<IEnumerable<AddressDto>>
    {
        [JsonProperty(Required = Required.Default)]
        public AddressFilterModel AddressModel { get; set; } = new AddressFilterModel();
    }

    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, IEnumerable<AddressDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAddressesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var address = _mapper.Map<Address>(request.AddressModel);

            var result = await _context.Bicycles
                .ByAddress(address)
                .AsNoTracking()
                .Select(x => x.RentalPointAddress)
                .ToListAsync(cancellationToken);

            return result.Distinct().Select(x => _mapper.Map<AddressDto>(x));
        }
    }
}

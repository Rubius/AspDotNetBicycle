using Application.Common.Dto;
using Application.Common.Exceptions;
using AutoMapper;
using Common.Services;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.EntitiesExtensions;
using Localization.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Rides.Commands.StartRide
{
    public class StartRideCommand : IRequest<long>
    {
        [JsonProperty(Required = Required.Always)]
        public long BicycleBrandId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public AddressDto AddressDto { get; set; } = new AddressDto();
    }

    public class TakeBicycleCommandHandler : IRequestHandler<StartRideCommand, long>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public TakeBicycleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<long> Handle(StartRideCommand request, CancellationToken cancellationToken)
        {
            var address = _mapper.Map<Address>(request.AddressDto);
            
            var bicycle = await _context.Bicycles.AsQueryable()
                .OnlyAvailableForRent(_context.Rides.AsQueryable())
                .ByAddress(address)
                .FirstOrDefaultAsync(cancellationToken);

            if (bicycle is null)
            {
                throw new BadRequestException(Resources.NoAvailableBicyclesForRent);
            }

            var ride = new Ride(_currentUserService.User!.Id, bicycle.Id);
            _context.Rides.Add(ride);

            await _context.SaveChangesAsync(cancellationToken);

            return ride.Id;
        }
    }
}

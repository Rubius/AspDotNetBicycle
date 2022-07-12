using Application.Common.Exceptions;
using Common.Services;
using Domain.Entities;
using Domain.EntitiesExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using SwaggerIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Application.Requests.Rides.Commands.FinishRide
{
    public class FinishRideCommand : IRequest<double>
    {
        [SwaggerIgnore]
        public long RideId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long Distance { get; set; }
    }

    public class ReturnBicycleCommandHandler : IRequestHandler<FinishRideCommand, double>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ReturnBicycleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<double> Handle(FinishRideCommand request, CancellationToken cancellationToken)
        {
            var ride = await _context.Rides
                .Include(x => x.Bicycle)
                    .ThenInclude(x => x!.Brand)
                .Where(x => x.Id == request.RideId)
                .FirstOrDefaultAsync(cancellationToken);

            if (ride is null)
            {
                throw new NotFoundException(nameof(Ride), request.RideId.ToString());
            }
            if (ride.Cost.HasValue)
            {
                return ride.Cost.Value;
            }

            var usersDistancesSum = _context.Rides.UserDistancesSum(_currentUserService.User!.Id);

            try
            {
                ride.Finish(usersDistancesSum, request.Distance);
            }
            catch (Exception ex)
            {
                Log.Error("Can`t calculate ride cost: id - {id}, {message} {ex}", request.RideId, ex.Message, ex);
                throw;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ride.Cost!.Value;
        }
    }
}

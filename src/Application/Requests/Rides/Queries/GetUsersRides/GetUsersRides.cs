using Application.Requests.Rides.Queries.GetUsersRides.Dto;
using AutoMapper;
using Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Rides.Queries
{
    public class GetUsersRidesQuery : IRequest<IEnumerable<UserRideDto>>
    {
    }

    public class GetUsersRidesQueryHandler : IRequestHandler<GetUsersRidesQuery, IEnumerable<UserRideDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetUsersRidesQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<UserRideDto>> Handle(GetUsersRidesQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Rides
                .Include(x => x.Bicycle)
                    .ThenInclude(x => x!.Model)
                .Where(x => x.UserId == _currentUserService.User!.Id)
                .Select(x => _mapper.Map<UserRideDto>(x))
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}

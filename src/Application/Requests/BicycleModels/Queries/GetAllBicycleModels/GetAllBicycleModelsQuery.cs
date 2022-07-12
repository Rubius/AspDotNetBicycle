using Application.Requests.BicycleModels.Queries.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.BicycleModels.Queries.GetAllBicycleModels;

public class GetAllBicycleModelsQuery : IRequest<IList<BicycleModelDto>>
{
}

public class GetAllBicycleModelsQueryHandler : IRequestHandler<GetAllBicycleModelsQuery, IList<BicycleModelDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBicycleModelsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<BicycleModelDto>> Handle(GetAllBicycleModelsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.BicycleModels
            .Select(x => _mapper.Map<BicycleModelDto>(x))
            .ToListAsync(cancellationToken);

        return result;
    }
}

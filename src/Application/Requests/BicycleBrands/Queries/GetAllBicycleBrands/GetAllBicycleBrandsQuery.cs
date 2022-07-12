using Application.Requests.BicycleBrands.Queries.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.BicycleBrands.Queries.GetAllBicycleBrands;

public class GetAllBicycleBrandsQuery : IRequest<IList<BicycleBrandDto>>
{
}

public class GetAllBicycleBrandsQueryHandler : IRequestHandler<GetAllBicycleBrandsQuery, IList<BicycleBrandDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBicycleBrandsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IList<BicycleBrandDto>> Handle(GetAllBicycleBrandsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.BicycleBrands
            .Select(x => _mapper.Map<BicycleBrandDto>(x))
            .ToListAsync(cancellationToken);

        return result;
    }
}

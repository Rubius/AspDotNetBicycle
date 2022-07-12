using Application.Common.Exceptions;
using Application.Common.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Application.Requests.BicycleBrands.Commands.UpdateBicycleBrand;

public class UpdateBicycleBrandCommand : IRequest, IMapTo<BicycleBrand>
{
    [SwaggerIgnore]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; } = "";

    [JsonProperty(Required = Required.Always)]
    public int LifeTimeYears { get; set; }

    [JsonProperty(Required = Required.Always)]
    public BicycleBrandClass Class { get; set; }

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<UpdateBicycleBrandCommand, BicycleBrand>()
            .ForMember(x => x.Bicycles, y => y.Ignore())
            .ForMember(x => x.ManufacturerAddress, y => y.Ignore());
    }
}

public class UpdateBicycleBrandsCommandHandler : IRequestHandler<UpdateBicycleBrandCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBicycleBrandsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateBicycleBrandCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BicycleBrands.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BicycleBrand), request.Id.ToString());
        }
        _mapper.Map(request, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
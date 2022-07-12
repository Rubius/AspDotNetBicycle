using Application.Common.Exceptions;
using Application.Common.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Application.Requests.BicycleModels.Commands.UpdateBicycleModel;

public class UpdateBicycleModelCommand : IRequest, IMapTo<BicycleModel>
{
    [SwaggerIgnore]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; } = "";

    [JsonProperty(Required = Required.Always)]
    public int LifeTimeYears { get; set; }

    [JsonProperty(Required = Required.Always)]
    public BicycleModelClass Class { get; set; }

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<UpdateBicycleModelCommand, BicycleModel>()
            .ForMember(x => x.Bicycles, y => y.Ignore())
            .ForMember(x => x.ManufacturerAddress, y => y.Ignore());
    }
}

public class UpdateBicycleModelsCommandHandler : IRequestHandler<UpdateBicycleModelCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBicycleModelsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateBicycleModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BicycleModels.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BicycleModel), request.Id.ToString());
        }
        _mapper.Map(request, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
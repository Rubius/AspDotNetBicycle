using Application.Common.Dto;
using Application.Common.Mapping;
using AutoMapper;
using Common.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.BicycleModels.Commands.CreateBicycleModel;

public class CreateBicycleModelCommand : IRequest<ulong>, IMapTo<BicycleModel>
{
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Name { get; set; } = "";

    [JsonProperty(Required = Required.Always)]
    public int LifeTimeYears { get; set; }

    [JsonProperty(Required = Required.Always)]
    public BicycleModelClass Class { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto ManufacturerAddress { get; set; } = new();

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<CreateBicycleModelCommand, BicycleModel>()
            .ForMember(x => x.Bicycles, y => y.Ignore())
            .ForMember(x => x.Id, y => y.Ignore());
    }
}

public class CreateBicycleModelsCommandHandler : IRequestHandler<CreateBicycleModelCommand, ulong>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBicycleModelsCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ulong> Handle(CreateBicycleModelCommand request, CancellationToken cancellationToken)
    {
        var row = _mapper.Map<BicycleModel>(request);
        _context.BicycleModels.Add(row);
        await _context.SaveChangesAsync(cancellationToken);

        return row.Id;
    }
}
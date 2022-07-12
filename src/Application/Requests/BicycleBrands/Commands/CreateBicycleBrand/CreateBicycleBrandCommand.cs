using Application.Common.Dto;
using Application.Common.Mapping;
using AutoMapper;
using Common.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.BicycleBrands.Commands.CreateBicycleBrand;

public class CreateBicycleBrandCommand : IRequest<ulong>, IMapTo<BicycleBrand>
{
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Name { get; set; } = "";

    [JsonProperty(Required = Required.Always)]
    public int LifeTimeYears { get; set; }

    [JsonProperty(Required = Required.Always)]
    public BicycleBrandClass Class { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto ManufacturerAddress { get; set; } = new();

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<CreateBicycleBrandCommand, BicycleBrand>()
            .ForMember(x => x.Bicycles, y => y.Ignore())
            .ForMember(x => x.Id, y => y.Ignore());
    }
}

public class CreateBicycleBrandsCommandHandler : IRequestHandler<CreateBicycleBrandCommand, ulong>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBicycleBrandsCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ulong> Handle(CreateBicycleBrandCommand request, CancellationToken cancellationToken)
    {
        var row = _mapper.Map<BicycleBrand>(request);
        _context.BicycleBrands.Add(row);
        await _context.SaveChangesAsync(cancellationToken);

        return row.Id;
    }
}
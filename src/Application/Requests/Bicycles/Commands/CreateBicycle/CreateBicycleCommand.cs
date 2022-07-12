using Application.Common.Dto;
using Application.Common.Mapping;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Commands.CreateBicycle;

public class CreateBicycleCommand : IRequest<ulong>, IMapTo<Bicycle>
{
    [JsonProperty(Required = Required.Always)]
    public ulong ModelId { get; set; }

    [JsonProperty(Required = Required.Always)]
    public DateTime ManufactureDate { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool IsWrittenOff { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto RentalPointAddress { get; set; } = new ();

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<CreateBicycleCommand, Bicycle>()
            .ForMember(x => x.Id, y => y.Ignore())
            .ForMember(x => x.Mileage, y => y.Ignore())
            .ForMember(x => x.NeedTechnicalInspection, y => y.Ignore())
            .ForMember(x => x.Rides, y => y.Ignore())
            .ForMember(x => x.TechnicalStatus, y => y.Ignore())
            .ForMember(x => x.Model, y => y.Ignore());
    }
}

public class CreateBicycleCommandHandler : IRequestHandler<CreateBicycleCommand, ulong>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBicycleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ulong> Handle(CreateBicycleCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Bicycle>(request);
        _context.Bicycles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

using Application.Common.Dto;
using Application.Common.Exceptions;
using Application.Common.Mapping;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Application.Requests.Bicycles.Commands.UpdateBicycle;

public class UpdateBicycleCommand : IRequest, IMapTo<Bicycle>
{
    [SwaggerIgnore]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public ulong BrandId { get; set; }

    [JsonProperty(Required = Required.Always)]
    public DateTime ManufactureDate { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool IsWrittenOff { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto RentalPointAddress { get; set; } = new ();

    public void MappingTo(Profile profile)
    {
        profile.CreateMap<UpdateBicycleCommand, Bicycle>()
            .ForMember(x => x.Brand, y => y.Ignore())
            .ForMember(x => x.Mileage, y => y.Ignore())
            .ForMember(x => x.NeedTechnicalInspection, y => y.Ignore())
            .ForMember(x => x.Rides, y => y.Ignore())
            .ForMember(x => x.TechnicalStatus, y => y.Ignore());
    }
}

public class UpdateBicycleBrandsCommandHandler : IRequestHandler<UpdateBicycleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBicycleBrandsCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateBicycleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Bicycles.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Bicycle), request.Id.ToString());
        }
        _mapper.Map(request, entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
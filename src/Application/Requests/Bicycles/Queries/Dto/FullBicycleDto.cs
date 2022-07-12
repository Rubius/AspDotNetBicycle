using Application.Common.Dto;
using Application.Common.Mapping;
using Application.Requests.BicycleModels.Queries.Dto;
using Domain.Entities;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.Dto;

public class FullBicycleDto : IMapFrom<Bicycle>
{
    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public DateTime ManufactureDate { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool IsWrittenOff { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto RentalPointAddress { get; set; } = new ();

    [JsonProperty(Required = Required.Always)]
    public BicycleModelDto Model { get; set; } = new();
}
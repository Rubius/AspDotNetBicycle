using Application.Common.Mapping;
using Domain.Entities;
using Domain.Enums;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.Dto;

public class ShortBicycleDto : IMapFrom<Bicycle>
{
    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    public DateTime ManufactureDate { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool IsWrittenOff { get; set; }

    [JsonProperty(Required = Required.Always)]
    public ulong ModelId { get; set; }

    [JsonProperty(Required = Required.Always)]
    public long Mileage { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool NeedTechnicalInspection { get; set; }

    [JsonProperty(Required = Required.Always)]
    public BicycleTechnicalStatus TechnicalStatus { get; set; }
}
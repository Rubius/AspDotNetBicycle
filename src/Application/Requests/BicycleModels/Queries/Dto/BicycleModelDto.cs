using Application.Common.Dto;
using Application.Common.Mapping;
using Common.Constants;
using Domain.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.BicycleModels.Queries.Dto;

public class BicycleModelDto : IMapFrom<BicycleModel>
{
    [JsonProperty(Required=Required.Always)]
    public ulong Id { get; set; }

    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Name { get; set; } = "";

    [JsonProperty(Required = Required.Always)]
    public int LifeTimeYears { get; set; }

    [JsonProperty(Required = Required.Always)]
    public AddressDto ManufacturerAddress { get; set; } = new();
}
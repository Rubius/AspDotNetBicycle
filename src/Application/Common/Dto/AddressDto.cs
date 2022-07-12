using Application.Common.Mapping;
using Common.Constants;
using Domain.Entities.ValueObjects;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Dto;

public class AddressDto : IMapFrom<Address>, IMapTo<Address>
{
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Country { get; set; } = string.Empty;

    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Region { get; set; } = string.Empty;

    [JsonProperty(Required = Required.AllowNull)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? City { get; set; }

    [JsonProperty(Required = Required.AllowNull)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? Street { get; set; }
}
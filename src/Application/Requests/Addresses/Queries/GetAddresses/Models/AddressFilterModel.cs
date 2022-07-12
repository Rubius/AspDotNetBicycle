using Application.Common.Mapping;
using Common.Constants;
using Domain.Entities.ValueObjects;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Addresses.Queries.GetAddresses.Models;

public class AddressFilterModel : IMapFrom<Address>, IMapTo<Address>
{
    [JsonProperty(Required = Required.Default)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? Country { get; set; }

    [JsonProperty(Required = Required.Default)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? Region { get; set; }

    [JsonProperty(Required = Required.Default)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? City { get; set; }

    [JsonProperty(Required = Required.Default)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string? Street { get; set; }
}

using Common.Constants;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Addresses.Queries.GetAvailableBicycles.Dto
{
    public class BicycleBrandCountDto
    {
        [JsonProperty(Required = Required.Always)]
        public ulong Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        [MaxLength(StringConstants.ShortTextLength)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public long Count { get; set; }
    }
}

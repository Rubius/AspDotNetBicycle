using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Queries.Dto
{
    public class BicycleTimeToBeWrittenOffDto
    {
        [JsonProperty(Required = Required.Always)]
        public ulong Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public TimeSpan TimeToBeWrittenOff { get; set; }
    }
}

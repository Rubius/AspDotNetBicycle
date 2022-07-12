using Application.Common.Mapping;
using AutoMapper;
using Common.Constants;
using Domain.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Rides.Queries.GetUsersRides.Dto
{
    public class UserRideDto : IMapFrom<Ride>
    {
        [JsonProperty(Required = Required.Always)]
        [MaxLength(StringConstants.ShortTextLength)]
        public string BicycleBrandName { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public DateTime StartDateTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime FinishDateTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long Distance { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double Cost { get; set; }

        public void MappingFrom(Profile profile)
        {
            profile.CreateMap<Ride, UserRideDto>().ForMember(x => x.BicycleBrandName, 
                opts => opts.MapFrom(y => y.Bicycle != null && y.Bicycle.Brand != null ? y.Bicycle.Brand.Name : string.Empty));
        }
    }
}

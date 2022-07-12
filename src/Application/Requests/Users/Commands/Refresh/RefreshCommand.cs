using Common.Constants;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Users.Commands.Refresh;

public class RefreshCommand
{
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string RefreshToken { get; set; } = string.Empty;
}
using Common.Models.Users;
using Newtonsoft.Json;

namespace Common.Models.Auth;

public class PermissionItem
{
    [JsonProperty(Required = Required.Always)]
    public Permission Permission { get; set; }

    [JsonProperty(Required = Required.Always)]
    public bool HasAccess { get; set; }
}
using Common.Models.Users;
using Newtonsoft.Json;

namespace Common.Models.Auth;

public class AuthResult
{
    [JsonProperty(Required = Required.Always)]
    public User User { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string AccessToken { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string RefreshToken { get; set; }

    [JsonProperty(Required = Required.Always)]
    public TimeSpan ExpiredIn { get; set; }

    public AuthResult(string accessToken, string refreshToken, TimeSpan expiredIn, User user)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiredIn = expiredIn;
        User = user;
    }
}
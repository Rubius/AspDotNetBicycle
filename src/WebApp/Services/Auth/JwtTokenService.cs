using Common.Models.Auth;
using Common.Models.Users;
using Common.Services;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace WebApp.Services.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateJwtToken(User user)
    {
        var permissionsString = string.Join('|', user.Permissions);

        var claims = new[]
        {
            new Claim(ClaimTypes.WindowsAccountName, user.Login),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
            new Claim(ClaimTypes.UserData, permissionsString),
        };

        Log.Information($@"Claim Created.
                             Login: {user.Login}
                             Permissions: {{Permissions}}
                             Name: '{user.Name}' 
                             Identifier: '{user.Id}'", user.Permissions);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.Add(_jwtSettings.AccessTokenLifetimeMinutes),
            signingCredentials: _jwtSettings.SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
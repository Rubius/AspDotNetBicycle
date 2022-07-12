using Application.Common.Exceptions;
using Common.Constants;
using Common.Models.Auth;
using Common.Models.Users;
using Common.Services;
using Localization.Resources;
using MediatR;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Users.Commands.Login;

public class LoginCommand : IRequest<LoginCommandResult>
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Пароль
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    [MaxLength(StringConstants.ShortTextLength)]
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly IAuthenticatorService _authenticatorService;

    public LoginCommandHandler(IAuthenticatorService authenticatorService)
    {
        _authenticatorService = authenticatorService;
    }

    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ExternalAuthUser? user;
        try
        {
            user = await _authenticatorService.Authenticate(request.Login, request.Password);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }

        if (user is null)
        {
            throw new BadRequestException(Resources.InvalidLoginOrPassword);
        }

        if (!user.Permissions.Any())
        {
            throw new BadRequestException(string.Format(Resources.UserDoesntContainRoles, request.Login));
        }

        var userObj = new User
        {
            Id = user.Id,
            Name = user.DisplayName,
            Email = user.Email,
            Login = request.Login,
            Permissions = user.Permissions.ToList()
        };

        return new LoginCommandResult
        {
            User = userObj
        };
    }
}
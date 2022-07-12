using Application.Requests.Users.Commands.Login;
using Application.Requests.Users.Commands.Refresh;
using Application.Requests.Users.Services;
using Common.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// API для работы с пользователя
/// </summary>
[Route("api/users")]
public class UsersController : ApiController
{
    private readonly ILoginService _loginService;

    public UsersController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// Войти в систему
    /// </summary>
    /// <param name="command">Учетные данные пользователя</param>
    /// <returns>Токены доступа в систему и данные о пользователе</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginCommand command)
    {
        var userData = await Mediator.Send(command);
        var result = await _loginService.Login(userData.User, HttpContext);

        return Ok(result);
    }

    /// <summary>
    /// Обновить токен доступа
    /// </summary>
    /// <param name="command">Токен обновления</param>
    /// <returns>Новая пара токенов для доступа в систему</returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResult>> Refresh([FromBody] RefreshCommand command)
    {
        var result = await _loginService.Refresh(command.RefreshToken, HttpContext);

        return Ok(result);
    }

    /// <summary>
    /// Выйти из системы
    /// </summary>
    /// <returns>Результат успешности выхода из системы</returns>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        _loginService.Logout(HttpContext);

        return Ok();
    }
}
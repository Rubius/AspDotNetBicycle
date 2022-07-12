using Application.Requests.Service.Command;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using WebApp.Filters;

namespace WebApp.Controllers;

/// <summary>
/// Служебное API 
/// </summary>
[Route("api/service")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class ServiceController : ApiController
{
    /// <summary>
    /// Инициализация БД начальными данными
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Auth(Permission.ServiceManage)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedDb()
    {
        await Mediator.Send(new SeedDbCommand());
        return Ok();
    }
}

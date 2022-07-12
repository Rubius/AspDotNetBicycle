using Application.Requests.BicycleBrands.Commands.CreateBicycleBrand;
using Application.Requests.BicycleBrands.Commands.DeleteBicycleBrand;
using Application.Requests.BicycleBrands.Commands.UpdateBicycleBrand;
using Application.Requests.BicycleBrands.Queries.Dto;
using Application.Requests.BicycleBrands.Queries.GetAllBicycleBrands;
using Application.Requests.BicycleBrands.Queries.GetBicycleBrand;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApp.Filters;

namespace WebApp.Controllers;

/// <summary>
/// API работы с моделями велосипедов
/// </summary>
[Route("api/bicycle-brands")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class BicycleBrandsController : ApiController
{
    /// <summary>
    /// Получить все модели велосипедов в системе
    /// </summary>
    /// <returns>Итерируемый список моделей велосипедов</returns>
    [HttpGet]
    [Auth(Permission.BicycleBrandRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<BicycleBrandDto>> GetAll() => await Mediator.Send(new GetAllBicycleBrandsQuery());

    /// <summary>
    ///  Получить информацию о конкретной модели велосипеда
    /// </summary>
    /// <param name="id">Идентификатор модели велосипеда в системе</param>
    /// <returns>Информацию о конкретной модели велосипеда</returns>
    [HttpGet("{id}")]
    [Auth(Permission.BicycleBrandRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BicycleBrandDto>> Get(ulong id)
        => await Mediator.Send(new GetBicycleBrandQuery(id));

    /// <summary>
    /// Добавить новую модель велосипеда
    /// </summary>
    /// <param name="command">Данные для инициализации новой модели велосипеда</param>
    /// <returns>Идентификатор созданной модели велосипеда в системе</returns>
    [HttpPost]
    [Auth(Permission.BicycleBrandCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ulong>> Create([FromBody] CreateBicycleBrandCommand command)
    {
        var bicycleBrandId = await Mediator.Send(command);
        var objectUrl = Url.Action(nameof(Get), new { id = bicycleBrandId });
        return Created(objectUrl!, bicycleBrandId);
    }

    /// <summary>
    /// Обновить информацию о модели велосипеда
    /// </summary>
    /// <param name="id">Идентификатор модели велосипеда в системе</param>
    /// <param name="command">Данные для обновления</param>
    /// <returns>Результат успешности выполнения обновления</returns>
    [HttpPut("{id}")]
    [Auth(Permission.BicycleBrandEdit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateBicycleBrandCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Удалить модель велосипеда
    /// </summary>
    /// <param name="id">Идентификатор модели велосипеда в системе</param>
    /// <returns>Результат успешности выполнения удаления</returns>
    [HttpDelete("{id}")]
    [Auth(Permission.BicycleBrandDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(ulong id)
    {
        await Mediator.Send(new DeleteBicycleBrandCommand(id));

        return NoContent();
    }
}
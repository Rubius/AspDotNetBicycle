using Application.Requests.BicycleModels.Commands.CreateBicycleModel;
using Application.Requests.BicycleModels.Commands.DeleteBicycleModel;
using Application.Requests.BicycleModels.Commands.UpdateBicycleModel;
using Application.Requests.BicycleModels.Queries.Dto;
using Application.Requests.BicycleModels.Queries.GetAllBicycleModels;
using Application.Requests.BicycleModels.Queries.GetBicycleModel;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApp.Filters;

namespace WebApp.Controllers;

/// <summary>
/// API работы с моделями велосипедов
/// </summary>
[Route("api/bicycle-models")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class BicycleModelsController : ApiController
{
    /// <summary>
    /// Получить все модели велосипедов в системе
    /// </summary>
    /// <returns>Итерируемый список моделей велосипедов</returns>
    [HttpGet]
    [Auth(Permission.BicycleModelRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<BicycleModelDto>> GetAll() => await Mediator.Send(new GetAllBicycleModelsQuery());

    /// <summary>
    ///  Получить информацию о конкретной модели велосипеда
    /// </summary>
    /// <param name="id">Идентификатор модели велосипеда в системе</param>
    /// <returns>Информацию о конкретной модели велосипеда</returns>
    [HttpGet("{id}")]
    [Auth(Permission.BicycleModelRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BicycleModelDto>> Get(ulong id)
        => await Mediator.Send(new GetBicycleModelQuery(id));

    /// <summary>
    /// Добавить новую модель велосипеда
    /// </summary>
    /// <param name="command">Данные для инициализации новой модели велосипеда</param>
    /// <returns>Идентификатор созданной модели велосипеда в системе</returns>
    [HttpPost]
    [Auth(Permission.BicycleModelCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ulong>> Create([FromBody] CreateBicycleModelCommand command)
    {
        var bicycleModelId = await Mediator.Send(command);
        var objectUrl = Url.Action(nameof(Get), new { id = bicycleModelId });
        return Created(objectUrl!, bicycleModelId);
    }

    /// <summary>
    /// Обновить информацию о модели велосипеда
    /// </summary>
    /// <param name="id">Идентификатор модели велосипеда в системе</param>
    /// <param name="command">Данные для обновления</param>
    /// <returns>Результат успешности выполнения обновления</returns>
    [HttpPut("{id}")]
    [Auth(Permission.BicycleModelEdit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateBicycleModelCommand command)
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
    [Auth(Permission.BicycleModelDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(ulong id)
    {
        await Mediator.Send(new DeleteBicycleModelCommand(id));

        return NoContent();
    }
}
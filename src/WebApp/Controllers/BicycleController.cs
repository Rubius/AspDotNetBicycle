using Application.Requests.Bicycles.Commands.CreateBicycle;
using Application.Requests.Bicycles.Commands.DeleteBicycle;
using Application.Requests.Bicycles.Commands.FinishBicycleTechnicalInspection;
using Application.Requests.Bicycles.Commands.UpdateBicycle;
using Application.Requests.Bicycles.Queries.Dto;
using Application.Requests.Bicycles.Queries.GetBicycle;
using Application.Requests.Bicycles.Queries.GetBicycles;
using Application.Requests.Bicycles.Queries.GetBicyclesTimeToBeWrittenOff;
using Application.Requests.Bicycles.Queries.GetBicyclesWillBeWrittenOffThisYear;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApp.Filters;

namespace WebApp.Controllers;

/// <summary>
/// API для работы с велосипедами
/// </summary>
[Route("api/bicycles")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class BicycleController : ApiController
{
    /// <summary>
    /// Получить все имеющиеся велосипеды
    /// </summary>
    /// <returns>Итерируемый список велосипедов без информации о модели</returns>
    [HttpPost("all-by-filters")]
    [Auth(Permission.BicycleRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<ShortBicycleDto>> GetBicycles(GetBicyclesQuery query) =>
        await Mediator.Send(query);

    /// <summary>
    /// Получить подробную информацию по конкретному велосипеду
    /// </summary>
    /// <param name="id">Идентификатор велосипеда</param>
    /// <returns>Данные о велосипеде при наличии</returns>
    [HttpGet("{id}")]
    [Auth(Permission.BicycleRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullBicycleDto>> Get(ulong id)
        => await Mediator.Send(new GetBicycleQuery(id));

    /// <summary>
    /// Создать новый велосипед
    /// </summary>
    /// <param name="command">Данные для инициализации велосипеда</param>
    /// <returns>Идентификатор созданного велосипеда в системе</returns>
    [HttpPost]
    [Auth(Permission.BicycleCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ulong>> Create([FromBody] CreateBicycleCommand command)
    {
        var bicycleId = await Mediator.Send(command);
        var objectUrl = Url.Action(nameof(Get), new { id = bicycleId });

        return Created(objectUrl!, bicycleId);
    }

    /// <summary>
    /// Обновить информацию о велосипеде
    /// </summary>
    /// <param name="id">Идентификатор велосипеда в системе</param>
    /// <param name="command">Данные для обновления</param>
    /// <returns>Результат успешности выполнения операции обновления</returns>
    [HttpPut("{id}")]
    [Auth(Permission.BicycleEdit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateBicycleCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Удалить велосипед
    /// </summary>
    /// <param name="id">Идентификатор велосипеда в системе</param>
    /// <returns>Результат успешности выполнения операции удаления</returns>
    [HttpDelete("{id}")]
    [Auth(Permission.BicycleDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(ulong id)
    {
        await Mediator.Send(new DeleteBicycleCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Получить велосипеды определенной модели, которые будут списаны до конца текущего года в конкретном городе
    /// </summary>
    /// <param name="query">Модель велосипеда и город проката</param>
    /// <returns>Список велосипедов, которые будут списаны до конца текущего года</returns>
    [HttpPost("written-off-this-year")]
    [Auth(Permission.BicycleRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IEnumerable<ShortBicycleDto>> GetBicyclesWillBeWrittenOffThisYear([FromBody] GetBicyclesWillBeWrittenOffThisYearQuery query)
        => Mediator.Send(query);


    /// <summary>
    /// Высчитать время до списания велосипедов в определенном городе проката
    /// </summary>
    /// <param name="query">Город проката велосипедов</param>
    /// <returns>Список велосипедов с промежутком времени до списания</returns>
    [HttpPost("written-off-time")]
    [Auth(Permission.BicycleRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IEnumerable<BicycleTimeToBeWrittenOffDto>> GetBicyclesTimeToBeWrittenOff(
        [FromBody] GetBicyclesTimeToBeWrittenOffQuery query)
        => Mediator.Send(query);

    /// <summary>
    /// Вернуть велосипед с ТО
    /// </summary>
    [HttpPut("{id}/technical-inspection")]
    [Auth(Permission.BicycleEdit)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task FinishBicycleTechnicalInspection(ulong id, FinishBicycleTechnicalInspectionCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
    }
}

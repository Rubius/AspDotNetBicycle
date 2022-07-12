using Application.Common.Dto;
using Application.Requests.Addresses.Queries.GetAddresses;
using Application.Requests.Addresses.Queries.GetAvailableBicycles;
using Application.Requests.Addresses.Queries.GetAvailableBicycles.Dto;
using Application.Requests.Rides.Commands.FinishRide;
using Application.Requests.Rides.Commands.StartRide;
using Application.Requests.Rides.Queries;
using Application.Requests.Rides.Queries.GetUsersRides.Dto;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using WebApp.Filters;

namespace WebApp.Controllers
{
    /// <summary>
    /// Поездки пользователей
    /// </summary>
    [Route("api/rides")]
    public class RidesController : ApiController
    {
        /// <summary>
        /// Получить адреса точек проката велосипедов
        /// </summary>
        /// <param name="query">Данные для поиска адресов</param>
        /// <returns>Список отфильтрованных адресов</returns>
        [HttpPost("rental-points-addresses")]
        [Auth(Permission.AddressRead)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<AddressDto>> GetRentalPointsAddresses(GetAddressesQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Получить количество доступных велосипедов имеющихся моделей
        /// </summary>
        /// <param name="query">Данные для поиска велосипедов</param>
        /// <returns>Список моделей велосипедов с количеством доступных для проката</returns>
        [HttpPost("rental-points-addresses/bicycle-models")]
        [Auth(Permission.AddressRead)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<BicycleModelCountDto>> GetRentalPointsAvailableBicycleModels(GetAvailableBicyclesQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Получить поездки пользователя
        /// </summary>
        /// <returns>Список поездок пользователя</returns>
        [HttpGet("by-user")]
        [Auth(Permission.RideRead)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<UserRideDto>> GetUserRides()
        {
            return await Mediator.Send(new GetUsersRidesQuery());
        }

        /// <summary>
        /// Взять велосипед в прокат
        /// </summary>
        /// <param name="command">Данные</param>
        /// <returns>Идентификатор поездки</returns>
        [HttpPost]
        [Auth(Permission.RideCreate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<long> StartRide(StartRideCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// Вернуть велосипед в прокат
        /// </summary>
        /// <param name="id">Идентификатор поездки</param>
        /// <param name="command">Данные о поездке</param>
        [HttpPut("{id}")]
        [Auth(Permission.RideEdit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<double> FinishRide(long id, FinishRideCommand command)
        {
            command.RideId = id;
            return await Mediator.Send(command);
        }
    }
}

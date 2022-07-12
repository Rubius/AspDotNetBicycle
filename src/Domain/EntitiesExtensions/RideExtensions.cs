using Domain.Entities;

namespace Domain.EntitiesExtensions
{
    public static class RideExtensions
    {
        /// <summary>
        /// Расстояние, пройденное пользователем за все время
        /// </summary>
        /// <param name="query">Запрос для поездок пользователей</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Сумма пройденных пользователем миль</returns>
        public static long UserDistancesSum(this IQueryable<Ride> query, Guid userId)
        {
            return query.Where(x => x.UserId == userId && x.Distance != null).Sum(x => x.Distance!.Value);
        }
    }
}

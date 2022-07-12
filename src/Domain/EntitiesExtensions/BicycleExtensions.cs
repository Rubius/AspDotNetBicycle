using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.Enums;
using System.Linq.Expressions;

namespace Domain.EntitiesExtensions
{
    public static class BicycleExtensions
    {
        /// <summary>
        /// Велосипеды, доступные для аренды
        /// </summary>
        /// <param name="query">Набор велосипедов</param>
        /// <returns>Отфильтрованный набор велосипедов</returns>
        public static IQueryable<Bicycle> WithGoodTechnicalStatus(this IQueryable<Bicycle> query)
        {
            Expression<Func<Bicycle, bool>> filter = x =>
                !x.IsWrittenOff
                && !x.NeedTechnicalInspection
                && x.TechnicalStatus == BicycleTechnicalStatus.ReadyToRide;

            return query.Where(filter);
        }

        /// <summary>
        /// Велосипеды, доступные для аренды
        /// </summary>
        /// <param name="query">Набор велосипедов</param>
        /// <returns>Отфильтрованный набор велосипедов, у которых НЕТ поездок ИЛИ НЕТ неоконченных поездок</returns>
        public static IQueryable<Bicycle> OnlyAvailableForRent(this IQueryable<Bicycle> bicycles, IQueryable<Ride> rides)
        {
            var bicyclesWithRidesJoin = from b in bicycles.WithGoodTechnicalStatus()
                        from r in rides.Where(r => b.Id == r.BicycleId).DefaultIfEmpty()
                        select new { b, r };

            var availableBicyclesIds = bicyclesWithRidesJoin.GroupBy(x => x.b.Id)
                    .Select(x => new
                    {
                        x.Key,
                        StartDateTime = x.Min(y => y.r != null ? y.r.StartDateTime : (DateTime?)null),
                        FinishDateTime = x.Min(y => y.r != null ? y.r.FinishDateTime : null)
                    })
                    .Where(x => x.StartDateTime == null || x.FinishDateTime != null)
                    .Select(x => x.Key);

            var filteredBicycles = from b in bicycles
                      join id in availableBicyclesIds on b.Id equals id
                      select b;

            return filteredBicycles;
        }

        /// <summary>
        /// Велосипеды, отфильтрованные по адресу проката
        /// </summary>
        /// <param name="query">Набор велосипедов</param>
        /// <param name="chosenAddress">Адрес для поиска</param>
        /// <returns>Отфильтрованный набор велосипедов по адресу</returns>
        public static IQueryable<Bicycle> ByAddress(this IQueryable<Bicycle> query, Address? chosenAddress)
        {
            if (chosenAddress is null)
            {
                return query;
            }

            return query.Where(x => (chosenAddress.Country == null || x.RentalPointAddress.Country == chosenAddress.Country)
                && (chosenAddress.Region == null || x.RentalPointAddress.Region == chosenAddress.Region)
                && (chosenAddress.City == null || x.RentalPointAddress.City == chosenAddress.City)
                && (chosenAddress.Street == null || x.RentalPointAddress.Street == chosenAddress.Street));
        }
    }
}

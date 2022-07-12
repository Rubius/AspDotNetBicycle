using Domain.Entities.ValueObjects;
using Domain.Enums;
using Domain.Exceptions;
using Localization.Resources;

namespace Domain.Entities;

public class Bicycle
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Дата производства
    /// </summary>
    public DateTime ManufactureDate { get; set; }

    /// <summary>
    /// Списан
    /// </summary>
    public bool IsWrittenOff { get; set; }

    /// <summary>
    /// Идентификатор модели
    /// </summary>
    public ulong ModelId { get; set; }

    /// <summary>
    /// Модель велосипеда
    /// </summary>
    public BicycleModel? Model { get; set; }

    /// <summary>
    /// Пробег велосипеда (мили)
    /// </summary>
    public long Mileage { get; protected set; }

    /// <summary>
    /// Нуждается в техническом осмотре
    /// </summary>
    public bool NeedTechnicalInspection { get; protected set; }

    /// <summary>
    /// Работоспособность велосипеда
    /// </summary>
    public BicycleTechnicalStatus TechnicalStatus { get; protected set; }

    /// <summary>
    /// Поездки пользователей
    /// </summary>
    public IEnumerable<Ride> Rides { get; set; } = new List<Ride>();

    /// <summary>
    /// Адрес проката
    /// </summary>
    private Address _rentalPointAddress;
    public Address RentalPointAddress { 
        get => _rentalPointAddress;
        protected set
        {
            if (value is null)
                throw new MappingValidationException(Resources.ArgumentNullOrEmptyError, nameof(RentalPointAddress), nameof(Bicycle));
            _rentalPointAddress = value;
        }
    }

#pragma warning disable CS8618
    public Bicycle(Address rentalPointAddress)
    {
        ChangeRentalPointAddress(rentalPointAddress);
        TechnicalStatus = BicycleTechnicalStatus.ReadyToRide;
    }

    protected Bicycle() { }
#pragma warning restore CS8618

    public void ChangeRentalPointAddress(Address rentalPointAddress)
    {
        RentalPointAddress = rentalPointAddress;
    }

    /// <summary>
    /// Вычисляет время до списания велосипеда с какой-то даты
    /// </summary>
    /// <param name="nowDateTime">Дата, с которой высчитывается интервал до списаниия</param>
    /// <returns>Интервал времени до списания</returns>
    public TimeSpan CalculateTimeToWriteOff(DateTime nowDateTime)
    {
        // день, когда велосипед надо списать
        var writeOffDateTime = new DateTime(
            ManufactureDate.Year + (Model?.LifeTimeYears ?? 0),
            ManufactureDate.Month,
            ManufactureDate.Day);

        // разница между днём списания и текущей датой и есть результат
        return writeOffDateTime - nowDateTime;
    }

    /// <summary>
    /// Фильтрация велосипедов по городу проката
    /// </summary>
    /// <param name="query">Список велосипедов</param>
    /// <param name="city">Город проката</param>
    /// <returns>Отфильтрованный список велосипедов</returns>
    public static IQueryable<Bicycle> FilterByCity(IQueryable<Bicycle> query, string? city)
    {
        if (string.IsNullOrEmpty(city?.Trim()))
        {
            return query;
        }
        return query.Where(x => x.RentalPointAddress.City != null
                           && x.RentalPointAddress.City.ToLower().Contains(city.ToLower()));
    }

    /// <summary>
    /// Добавить мили пробега
    /// </summary>
    /// <param name="miles">Количество миль</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddMileage(long miles)
    {
        if (miles < 0)
        {
            throw new InvalidOperationException("Can`t increase" + nameof(Mileage) + "by negative number.");
        }

        var oldMileage = Mileage;
        Mileage += miles;
        if ((oldMileage % MilesForTechnicalInspection) + miles >= MilesForTechnicalInspection)
        {
            NeedTechnicalInspection = true;
        }
    }

    private const int MilesForTechnicalInspection = 100;

    /// <summary>
    /// Завершить тех. осмотр
    /// </summary>
    /// <param name="result">Систояние велосипеда по результатам тех. осмотра</param>
    public void FinishTechnicalInspection(BicycleTechnicalStatus result)
    {
        NeedTechnicalInspection = false;

        // уже списанный велосипед не может быть возвращен в оборот
        if (!(TechnicalStatus == BicycleTechnicalStatus.Decommissioned))
        {
            TechnicalStatus = result;
        }
    }
}

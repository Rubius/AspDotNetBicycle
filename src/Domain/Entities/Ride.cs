using Domain.Rides;

namespace Domain.Entities;

/// <summary>
/// Сущность поездки пользователей на велосипедах
/// </summary>
public class Ride
{
    public Ride(Guid userId, ulong bicycleId)
    {
        UserId = userId;
        BicycleId = bicycleId;
        StartDateTime = DateTime.Now;
    }

    protected Ride() { }

    public long Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Идентификатор велосипеда
    /// </summary>
    public ulong BicycleId { get; private set; }

    /// <summary>
    /// Велосипед
    /// </summary>
    public Bicycle? Bicycle { get; private set; }

    /// <summary>
    /// Время начала проката
    /// </summary>
    public DateTime StartDateTime { get; private set; }

    /// <summary>
    /// Время окончания проката
    /// </summary>
    public DateTime? FinishDateTime { get; private set; }

    /// <summary>
    /// Пройденое расстояние (мили)
    /// </summary>
    public long? Distance { get; private set; }

    /// <summary>
    /// Стоимость поездки
    /// </summary>
    public double? Cost { get; private set; }

    /// <summary>
    /// Установить пройденное расстояние
    /// </summary>
    /// <param name="distance">Расстояние</param>
    /// <exception cref="ArgumentException">Не загружены связанные данные</exception>
    internal void SetDistance(long distance)
    {
        if (distance < 0)
        {
            throw new ArgumentException("Can`t be less than 0", nameof(distance));
        }
        Distance = distance;
        FinishDateTime = DateTime.Now;
    }

    /// <summary>
    /// Завершить прокат велосипеда
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="distance">Пройденное расстояние</param>
    /// <returns>Стоимость поездки</returns>
    public double Finish(long usersDistancesSum, long distance)
    {
        SetDistance(distance);
        IncreaseBicycleMileage(distance);
        CalculateCost(usersDistancesSum);

        return Cost ?? throw new Exception("Can`t calculate ride cost");
    }

    /// <summary>
    /// Добавить пробег велосипеду после проката
    /// </summary>
    /// <param name="distance">Количество пройденных миль</param>
    /// <exception cref="InvalidOperationException"></exception>
    private void IncreaseBicycleMileage(long distance)
    {
        if (Bicycle is null)
        {
            throw new InvalidOperationException($"Can`t increase mileage for a {nameof(Bicycle)}: {nameof(Bicycle)} is null");
        }
        Bicycle.AddMileage(distance);
    }

    /// <summary>
    /// Рассчитать стоимость поездки
    /// </summary>
    /// <param name="usersDistancesSum"></param>
    private void CalculateCost(long usersDistancesSum)
    {
        var calculator = new RideCostCalculator();
        Cost = calculator.Calculate(usersDistancesSum, this);
    }

    /// <summary>
    /// Длительность проката в часах
    /// </summary>
    /// <returns>Количество часов</returns>
    /// <exception cref="Exception"></exception>
    public double RideHours()
    {
        if (FinishDateTime is null || Distance is null)
        {
            throw new Exception("Can`t calculate ride cost");
        }
        var difference = FinishDateTime.Value - StartDateTime;

        return difference.TotalMinutes / 60D;
    }
}

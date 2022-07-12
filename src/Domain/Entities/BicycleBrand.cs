using Domain.Entities.ValueObjects;
using Domain.Enums;
using Domain.Exceptions;
using Localization.Resources;

namespace Domain.Entities;

public class BicycleBrand
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Название модели
    /// </summary>
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new MappingValidationException(Resources.ArgumentNullOrEmptyError, nameof(Name), nameof(BicycleBrand));
            }
            _name = value;
        }
    }

    /// <summary>
    /// Срок службы
    /// </summary>
    public int LifeTimeYears { get; set; }

    /// <summary>
    /// Велосипеды текущей модели
    /// </summary>
    public List<Bicycle>? Bicycles { get; set; }

    /// <summary>
    /// Адрес производства
    /// </summary>
    private readonly Address _manufacturerAddress;
    public Address ManufacturerAddress {
        get => _manufacturerAddress;
        protected init
        {
            if (value is null)
                throw new MappingValidationException(Resources.ArgumentNullOrEmptyError, nameof(ManufacturerAddress), nameof(BicycleBrand));
            _manufacturerAddress = value;
        }
    }

    public BicycleBrandClass Class { get; private set; } 

#pragma warning disable CS8618
    public BicycleBrand(string name, BicycleBrandClass brandClass, Address manufacturerAddress)
    {
        Name = name;
        Class = brandClass;
        ManufacturerAddress = manufacturerAddress;
    }

    protected BicycleBrand() { }
#pragma warning restore CS8618

    /// <summary>
    /// Велосипеды, которые нужно списать до определенной даты
    /// </summary>
    /// <param name="thisBrandBicycles">Велосипеды</param>
    /// <param name="writeOffDate">Время, до которого велосипед должен быть списан</param>
    /// <returns>Список велосипедов данной модели, которые нужно списать до определенной даты</returns>
    public IQueryable<Bicycle> GetBicyclesWillBeWrittenOff(IQueryable<Bicycle> thisBrandBicycles, DateTime writeOffDate)
    {
        // получаем IQueriable для несписанных велосипедов
        var nonWrittenOffBicycles = thisBrandBicycles
            .Where(x => x.BrandId == Id)
            .Where(x => x.IsWrittenOff == false);

        // списаны будут те велосипеды, дата списания которых раньше указанной даты списания
        var bicyclesWillBeWrittenOffThisYear = nonWrittenOffBicycles
            .Where(x => x.ManufactureDate.AddYears(LifeTimeYears) < writeOffDate);
        return bicyclesWillBeWrittenOffThisYear;
    }

    /// <summary>
    /// Велосипеды, которые нужно списать до конца года
    /// </summary>
    /// <param name="thisBrandBicycles">Велосипеды</param>
    /// <returns>Список велосипедов данной модели, которые нужно списать до конца года</returns>
    public IQueryable<Bicycle> GetBicyclesWillBeWrittenOffThisYear(IQueryable<Bicycle> thisBrandBicycles)
    {
        return GetBicyclesWillBeWrittenOff(thisBrandBicycles, new DateTime(DateTime.Now.Year + 1, 1, 1));
    }
}
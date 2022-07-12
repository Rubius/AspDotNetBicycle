using Domain.Abstractions;
using Domain.Exceptions;
using Localization.Resources;

namespace Domain.Entities.ValueObjects;

public class Address : ValueObject
{
    public Address(string country, string region, string? city = null, string? street = null)
    {
        Country = country;
        Region = region;
        City = city;
        Street = street;
    }

    protected Address() { }

    private readonly string _country = string.Empty;
    public string Country
    {
        get => _country;
        private init
        {
            if (string.IsNullOrEmpty(value))
                throw new MappingValidationException(Resources.ArgumentNullOrEmptyError, nameof(Country), nameof(Address));
            _country = value;
        }
    }

    private readonly string _region = string.Empty;
    public string Region
    {
        get => _region;
        private init
        {
            if (string.IsNullOrEmpty(value))
                throw new MappingValidationException(Resources.ArgumentNullOrEmptyError, nameof(Region), nameof(Address));
            _region = value;
        }
    }

    public string? City { get; private init; }
    public string? Street { get; private init; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return Region;
        yield return City;
        yield return Street;
    }
}
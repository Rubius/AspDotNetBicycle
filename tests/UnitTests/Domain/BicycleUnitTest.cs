using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public class BicycleUnitTest
{
    [Fact]
    public void OldBicycle_Should_BeWrittenOffSoon()
    {
        // arrange
        var model = new BicycleModel("BM-1254", BicycleModelClass.A, new Address("Russia", "Moscow"))
        {
            LifeTimeYears = 10
        };
        var bicycle = new Bicycle(new Address("USA", "California", "LA"))
        {
            ManufactureDate = new DateTime(2012, 1, 2),
            Model = model
        };

        // act
        var timeSpanToWriteOff = bicycle.CalculateTimeToWriteOff(new DateTime(2022, 1, 1));

        // assert
        timeSpanToWriteOff.TotalDays.Should().Be(1);
    }


    [Fact]
    public void Bicycle_Should_BeFilteredByCity()
    {
        // arrange
        var bicyclesQuery = new List<Bicycle>
        {
            new(new Address("Russia", "Tomsk", "Tomsk"))
            {
                ManufactureDate = new DateTime(2012, 1, 2),
            },
            new(new Address("Russia", "Tomsk", "Parabel"))
            {
                ManufactureDate = new DateTime(2014, 1, 2),
            }
        }.AsQueryable();
        const string rentalCity = "Tomsk";

        // act
        var filteredBicycles = Bicycle.FilterByCity(bicyclesQuery, rentalCity);

        // assert
        filteredBicycles.Count().Should().Be(1);
    }
}

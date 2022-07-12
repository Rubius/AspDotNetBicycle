using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public class BicycleModelUnitTest
{
    [Fact]
    public void OnlyOldBicycles_Should_Return_OneBicycle()
    {
        // arrange

        var address = new Address("USA", "LA");

        var model = new BicycleModel("Model-4", BicycleModelClass.A, address)
        {
            LifeTimeYears = 10
        };

        var nowYear = DateTime.Now.Year;

        var bicycles = new List<Bicycle>
        {
            new(address) { Id = 1, ManufactureDate = new DateTime(nowYear - 8, 3, 8), IsWrittenOff = false},
            new(address) { Id = 2, ManufactureDate = new DateTime(nowYear - 10, 1, 2), IsWrittenOff = false},
            new(address) { Id = 3, ManufactureDate = new DateTime(nowYear - 10, 1, 2), IsWrittenOff = true},
        };

        // act
        var bicyclesWillBeWrittenOffThisYear =
            model.GetBicyclesWillBeWrittenOffThisYear(bicycles.AsQueryable())
                .ToList();

        // assert
        bicyclesWillBeWrittenOffThisYear.Count.Should().Be(1);
        bicyclesWillBeWrittenOffThisYear[0].Id.Should().Be(2);
    }
}
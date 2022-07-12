using Application.Common.Dto;
using Application.Requests.BicycleModels.Commands.CreateBicycleModel;
using Application.Requests.Bicycles.Commands.CreateBicycle;
using Application.Requests.Bicycles.Commands.UpdateBicycle;
using Application.Requests.Bicycles.Queries.GetBicycles;
using Application.Requests.Bicycles.Queries.GetBicyclesTimeToBeWrittenOff;
using Application.Requests.Bicycles.Queries.GetBicyclesWillBeWrittenOffThisYear;
using FluentAssertions;
using IntegrationTests.Abstractions;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Endpoints;

public class BicycleEndpointsTests : EndpointTest
{
    public BicycleEndpointsTests(WebAppFixture webAppFixture) : base(webAppFixture)
    {
    }

    [Fact]
    public async Task GetOne_Should_ThrowNotFound()
    {
        // act
        Func<Task> action = async () => await ApiClients.Bicycles.GetAsync(int.MaxValue);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetAll_Should_ReturnList()
    {
        // arrange
        var bicycleId = await CreateBicycleAsync();

        // act
        var result = await ApiClients.Bicycles.GetBicyclesAsync(new GetBicyclesQuery());

        // assert
        result.Count.Should().Be(1);
        result.ElementAt(0).Id.Should().Be(bicycleId);
    }

    [Fact]
    public async Task Create_Should_ThrowApiException()
    {
        // arrange
        var modelId = await ApiClients.BicycleModels.CreateAsync(CreateBicycleModelCommand);
        var incorrectCreateBicycleCommand = BuildCreateBicycleCommand((ulong)modelId, IncorrectAddressDto);

        // act
        Func<Task> action = async () => await ApiClients.Bicycles.CreateAsync(incorrectCreateBicycleCommand);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_Should_UpdateBicycle()
    {
        //arrange
        var bicycleId = await CreateBicycleAsync();
        var bicycle = await ApiClients.Bicycles.GetAsync((long)bicycleId);
        var guid = Guid.NewGuid().ToString();
        var newManufactureDate = bicycle.ManufactureDate.AddDays(10);
        var updateBicycleCommand = new UpdateBicycleCommand
        {
            ModelId = bicycle.Model.Id,
            IsWrittenOff = bicycle.IsWrittenOff,
            ManufactureDate = newManufactureDate,
            RentalPointAddress = new AddressDto
            {
                Country = guid,
                Region = guid,
            }
        };

        // act
        await ApiClients.Bicycles.UpdateAsync((long)bicycleId, updateBicycleCommand);

        // assert
        var updatedBicycle = await ApiClients.Bicycles.GetAsync((long)bicycleId);
        updatedBicycle.ManufactureDate.Should().Be(newManufactureDate);
        updatedBicycle.RentalPointAddress.Country.Should().Be(guid);
        updatedBicycle.RentalPointAddress.Region.Should().Be(guid);
    }

    [Fact]
    public async Task Delete_Should_DeleteBicycle()
    {
        //arrange
        var bicycleId = await CreateBicycleAsync();

        // act
        await ApiClients.Bicycles.DeleteAsync((long)bicycleId);

        // assert
        var bicyclesList = await ApiClients.Bicycles.GetBicyclesAsync(new GetBicyclesQuery());
        bicyclesList.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetBicyclesTimeToBeWrittenOff_Should_ReturnOne()
    {
        //arrange
        var bicycleId = await CreateBicycleAsync();
        var query = new GetBicyclesTimeToBeWrittenOffQuery
        {
            RentalPointCity = CorrectAddressDto.City ?? string.Empty
        };

        // act
        var result = await ApiClients.Bicycles.GetBicyclesTimeToBeWrittenOffAsync(query);

        // assert
        result.Should().HaveCount(1);
        result.ElementAt(0).Id.Should().Be(bicycleId);
        result.ElementAt(0).TimeToBeWrittenOff.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task GetBicyclesWillBeWrittenOffThisYear_Should_BeEmpty()
    {
        //arrange
        var modelId = await ApiClients.BicycleModels.CreateAsync(CreateBicycleModelCommand);
        await ApiClients.Bicycles.CreateAsync(BuildCreateBicycleCommand((ulong)modelId, CorrectAddressDto));
        var query = new GetBicyclesWillBeWrittenOffThisYearQuery
        {
            RentalPointCity = CorrectAddressDto.City ?? string.Empty,
            ModelId = (ulong)modelId
        };

        // act
        var result = await ApiClients.Bicycles.GetBicyclesWillBeWrittenOffThisYearAsync(query);

        // assert
        result.Should().HaveCount(0);
    }

    private async Task<ulong> CreateBicycleAsync()
    {
        var modelId = await ApiClients.BicycleModels.CreateAsync(CreateBicycleModelCommand);

        var bicycleId = await ApiClients.Bicycles.CreateAsync(BuildCreateBicycleCommand((ulong)modelId, CorrectAddressDto));
        return (ulong)bicycleId;
    }

    private static CreateBicycleModelCommand CreateBicycleModelCommand => new()
    {
        Name = "bicycleModel",
        ManufacturerAddress = CorrectAddressDto,
        LifeTimeYears = 2
    };

    private static CreateBicycleCommand BuildCreateBicycleCommand(ulong modelId, AddressDto address) => new()
    {
        ModelId = modelId,
        IsWrittenOff = true,
        ManufactureDate = DateTime.Now.AddYears(-1).AddMonths(-1),
        RentalPointAddress = address
    };
}
using Application.Requests.BicycleModels.Commands.CreateBicycleModel;
using Application.Requests.BicycleModels.Commands.UpdateBicycleModel;
using FluentAssertions;
using IntegrationTests.Abstractions;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Endpoints;

public class BicycleModelEndpointsTests : EndpointTest
{
    public BicycleModelEndpointsTests(WebAppFixture webAppFixture) : base(webAppFixture)
    {
    }

    [Fact]
    public async Task GetOne_Should_ThrowNotFound()
    {
        // act
        Func<Task> action = async () => await ApiClients.BicycleModels.GetAsync(int.MaxValue);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetAll_Should_ReturnList()
    {
        //arrange
        var modelId = await CreateBicycleModelAsync();

        // act
        var result = await ApiClients.BicycleModels.GetAllAsync();

        // assert
        result.Count.Should().Be(1);
        result.ElementAt(0).Id.Should().Be(modelId);
    }

    [Fact]
    public async Task Create_Should_ThrowApiException()
    {
        //arrang
        var incorrectCreateBicycleModelCommand = CreateBicycleModelCommand;
        incorrectCreateBicycleModelCommand.ManufacturerAddress = IncorrectAddressDto;

        // act
        Func<Task> action = async () => await ApiClients.BicycleModels.CreateAsync(incorrectCreateBicycleModelCommand);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_Should_UpdateBicycle()
    {
        //arrange
        var modelId = await CreateBicycleModelAsync();
        var model = await ApiClients.BicycleModels.GetAsync((long)modelId);
        var guid = Guid.NewGuid().ToString();
        var newLifeTimeYears = model.LifeTimeYears + 10;
        var updateBicycleModelCommand = new UpdateBicycleModelCommand
        {
            LifeTimeYears = newLifeTimeYears,
            Name = guid
        };

        // act
        await ApiClients.BicycleModels.UpdateAsync((long)modelId, updateBicycleModelCommand);

        // assert
        var updatedBicycle = await ApiClients.BicycleModels.GetAsync((long)modelId);
        updatedBicycle.Name.Should().Be(guid);
        updatedBicycle.LifeTimeYears.Should().Be(newLifeTimeYears);
    }

    [Fact]
    public async Task Delete_Should_DeleteBicycle()
    {
        //arrange
        var modelId = await CreateBicycleModelAsync();

        // act
        await ApiClients.BicycleModels.DeleteAsync((long)modelId);

        // assert
        var bicyclesList = await ApiClients.BicycleModels.GetAllAsync();
        bicyclesList.Should().HaveCount(0);
    }


    private async Task<ulong> CreateBicycleModelAsync()
    {
        var modelId = await ApiClients.BicycleModels.CreateAsync(CreateBicycleModelCommand);
        return (ulong)modelId;
    }

    private static CreateBicycleModelCommand CreateBicycleModelCommand => new()
    {
        Name = "bicycleModel",
        LifeTimeYears = 10,
        ManufacturerAddress = CorrectAddressDto
    };
}
using Application.Requests.BicycleBrands.Commands.CreateBicycleBrand;
using Application.Requests.BicycleBrands.Commands.UpdateBicycleBrand;
using FluentAssertions;
using IntegrationTests.Abstractions;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Endpoints;

public class BicycleBrandEndpointsTests : EndpointTest
{
    public BicycleBrandEndpointsTests(WebAppFixture webAppFixture) : base(webAppFixture)
    {
    }

    [Fact]
    public async Task GetOne_Should_ThrowNotFound()
    {
        // act
        Func<Task> action = async () => await ApiClients.BicycleBrands.GetAsync(int.MaxValue);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetAll_Should_ReturnList()
    {
        //arrange
        var brandId = await CreateBicycleBrandAsync();

        // act
        var result = await ApiClients.BicycleBrands.GetAllAsync();

        // assert
        result.Count.Should().Be(1);
        result.ElementAt(0).Id.Should().Be(brandId);
    }

    [Fact]
    public async Task Create_Should_ThrowApiException()
    {
        //arrang
        var incorrectCreateBicycleBrandCommand = CreateBicycleBrandCommand;
        incorrectCreateBicycleBrandCommand.ManufacturerAddress = IncorrectAddressDto;

        // act
        Func<Task> action = async () => await ApiClients.BicycleBrands.CreateAsync(incorrectCreateBicycleBrandCommand);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_Should_UpdateBicycle()
    {
        //arrange
        var brandId = await CreateBicycleBrandAsync();
        var brand = await ApiClients.BicycleBrands.GetAsync((long)brandId);
        var guid = Guid.NewGuid().ToString();
        var newLifeTimeYears = brand.LifeTimeYears + 10;
        var updateBicycleBrandCommand = new UpdateBicycleBrandCommand
        {
            LifeTimeYears = newLifeTimeYears,
            Name = guid
        };

        // act
        await ApiClients.BicycleBrands.UpdateAsync((long)brandId, updateBicycleBrandCommand);

        // assert
        var updatedBicycle = await ApiClients.BicycleBrands.GetAsync((long)brandId);
        updatedBicycle.Name.Should().Be(guid);
        updatedBicycle.LifeTimeYears.Should().Be(newLifeTimeYears);
    }

    [Fact]
    public async Task Delete_Should_DeleteBicycle()
    {
        //arrange
        var brandId = await CreateBicycleBrandAsync();

        // act
        await ApiClients.BicycleBrands.DeleteAsync((long)brandId);

        // assert
        var bicyclesList = await ApiClients.BicycleBrands.GetAllAsync();
        bicyclesList.Should().HaveCount(0);
    }


    private async Task<ulong> CreateBicycleBrandAsync()
    {
        var brandId = await ApiClients.BicycleBrands.CreateAsync(CreateBicycleBrandCommand);
        return (ulong)brandId;
    }

    private static CreateBicycleBrandCommand CreateBicycleBrandCommand => new()
    {
        Name = "bicycleBrand",
        LifeTimeYears = 10,
        ManufacturerAddress = CorrectAddressDto
    };
}
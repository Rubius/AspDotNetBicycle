using Application.Common.Mapping;
using Application.Requests.BicycleModels.Queries.Dto;
using Application.Requests.Bicycles.Queries.Dto;
using AutoFixture;
using AutoFixture.Kernel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;
    private readonly IFixture _fixture;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = _configuration.CreateMapper();

        _fixture = new Fixture();
        _fixture.Customize<BicycleModel>(x => x.Without(y => y.Bicycles));
    }

    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Theory]
    [InlineData(typeof(BicycleModel), typeof(BicycleModelDto))]
    [InlineData(typeof(Bicycle), typeof(FullBicycleDto))]
    [InlineData(typeof(Bicycle), typeof(ShortBicycleDto))]
    public void ShouldSupportMappingFromSourceToDestination3(Type source, Type destination)
    {
        var model = Create(source);
        _mapper.Map(model, source, destination);
    }

    private object Create(Type source)
    {
        var specimenFactoryType = typeof(SpecimenFactory);
        var method = specimenFactoryType
            .GetMethods().First(x => x.Name == "Create" 
                                     && x.GetParameters().First().ParameterType == typeof(ISpecimenBuilder));

        Type[] typeArgs = { source };
        var invokeMethod = method.MakeGenericMethod(typeArgs);

        var result = invokeMethod.Invoke(null, new object[] { _fixture });
        if (result is null)
        {
            throw new ObjectCreationException();
        }
        return result;
    }
}
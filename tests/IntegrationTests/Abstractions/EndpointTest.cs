using Application.Common.Dto;
using Domain.Entities;
using IntegrationTests.Fixtures;
using System.Threading.Tasks;
using Respawn.Graph;
using Xunit;

namespace IntegrationTests.Abstractions;

public abstract class EndpointTest : IClassFixture<WebAppFixture>, IAsyncLifetime
{
    private readonly WebAppFixture _webAppFixture;
    private readonly bool _withoutCookies;
    protected static AddressDto CorrectAddressDto => new()
    {
        Country = "Russia",
        Region = "Moscow",
        City = "Zelenograd"
    };

    protected static AddressDto IncorrectAddressDto => new();

#nullable disable
    protected ApiClientManager ApiClients { get; private set; }

    protected EndpointTest(WebAppFixture webAppFixture)
    {
        _webAppFixture = webAppFixture;
    }

    protected EndpointTest(WebAppFixture webAppFixture, bool withoutCookies)
    {
        _webAppFixture = webAppFixture;
        _withoutCookies = withoutCookies;
    }
#nullable enable
    public async Task InitializeAsync()
    {
        var tablesToExclude = new Table[] { nameof(UserRefreshToken)+"s" };
        await _webAppFixture.ResetDbState(tablesToExclude);

        var factory = _webAppFixture.Factory;
        ApiClients = new ApiClientManager(factory, _withoutCookies);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
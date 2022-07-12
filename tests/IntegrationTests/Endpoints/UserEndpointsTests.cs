using Application.Requests.Users.Commands.Login;
using FluentAssertions;
using IntegrationTests.Abstractions;
using IntegrationTests.Fixtures;
using IntegrationTests.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Endpoints;

public class UserEndpointsTests : EndpointTest
{
    public UserEndpointsTests(WebAppFixture webAppFixture) : base(webAppFixture, withoutCookies: true)
    {
    }

    [Fact]
    public async Task Login_Should_Ok()
    {
        // arrange
        var command = new LoginCommand
        {
            Login = ApiClients.TestUser.Login,
            Password = TestAuthenticatorService.CorrectPassword
        };

        // act
        var result = await ApiClients.Users.LoginAsync(command);

        // assert
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.User.Should().BeEquivalentTo(Users.GetUser());
    }


    [Fact]
    public async Task Login_Should_ThrowApiException()
    {
        // arrange
        var command = new LoginCommand
        {
            Login = ApiClients.TestUser.Login,
            Password = TestAuthenticatorService.IncorrectPassword
        };

        // act
        var action = async () => await ApiClients.Users.LoginAsync(command);

        // assert
        var exception = await action.Should().ThrowAsync<ApiException>();
        exception.Where(x => x.StatusCode == StatusCodes.Status400BadRequest);
    }
}
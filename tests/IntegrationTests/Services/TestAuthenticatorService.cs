using Common.Models.Auth;
using Common.Services;
using System.Threading.Tasks;

namespace IntegrationTests.Services;

public class TestAuthenticatorService : IAuthenticatorService
{
    public static string CorrectPassword => "correct_password";
    public static string IncorrectPassword => "incorrect_password";

    public Task<ExternalAuthUser?> Authenticate(string login, string password)
    {
        if (password != CorrectPassword)
        {
            return Task.FromResult((ExternalAuthUser?)null);
        }

        var user = Users.GetUser();
        return Task.FromResult(
            (ExternalAuthUser?) new ExternalAuthUser(user.Id, user.Name, user.Email, user.Permissions));
    }
}
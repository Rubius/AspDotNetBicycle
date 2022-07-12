using Common.Models.Auth;
using Common.Services;

namespace WebApp.Services.Mock;

public class MockAuthenticatorService : IAuthenticatorService
{
    public Task<ExternalAuthUser?> Authenticate(string login, string password)
    {
        foreach (var user in Users.UsersList)
        {
            if (IsEqual(user, login, password))
            {
                return Task.FromResult((ExternalAuthUser?)user);
            }
        }
        return Task.FromResult((ExternalAuthUser?)null);
    }

    private static bool IsEqual(ExternalAuthUser systemUser, string login, string password) =>
        systemUser.DisplayName == login && systemUser.DisplayName == password;
}
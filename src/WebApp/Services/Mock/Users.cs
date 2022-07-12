using Common.Models.Auth;
using Common.Models.Users;

namespace WebApp.Services.Mock;
public class Users
{
    internal static IEnumerable<ExternalAuthUser> UsersList => new List<ExternalAuthUser> { Admin, User };

    internal static ExternalAuthUser Admin => new(
        new Guid("7a6a2e01e779490c88bf7d7285cbadc3"),
        "admin",
        "",
        Enum.GetValues<Permission>());

    internal static ExternalAuthUser User => new(
        new Guid("23976fe931654a248cd65916086dada0"),
        "user",
        "",
        Enum.GetValues<Permission>().Where(x => x.ToString().Contains("Read")));
}
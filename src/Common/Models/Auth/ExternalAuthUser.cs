using Common.Models.Users;

namespace Common.Models.Auth;

public class ExternalAuthUser
{
    public ExternalAuthUser(Guid id, string displayName, string? email, IEnumerable<Permission> permissions)
    {
        Id = id;
        DisplayName = displayName;
        Email = email;
        Permissions = new List<Permission>(permissions);
    }

    public Guid Id { get; }
    public string DisplayName { get; }
    public string? Email { get; }
    public IReadOnlyList<Permission> Permissions { get; }
}
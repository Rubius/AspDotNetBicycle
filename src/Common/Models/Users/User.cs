namespace Common.Models.Users;

public class User
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public IEnumerable<Permission> Permissions { get; init; } = Array.Empty<Permission>();
}
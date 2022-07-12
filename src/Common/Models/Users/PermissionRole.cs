namespace Common.Models.Users;

public class PermissionRole
{
    public string Permission { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}
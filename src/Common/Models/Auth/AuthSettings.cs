namespace Common.Models.Auth;

public class AuthSettings
{
    public LdapConfig Ldap { get; init; } = new();
}
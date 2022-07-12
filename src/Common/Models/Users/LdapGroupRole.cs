namespace Common.Models.Users;

public class LdapGroupRole
{
    public string LdapGroup { get; set; } = string.Empty;
    public string SystemRole { get; set; } = string.Empty;
}
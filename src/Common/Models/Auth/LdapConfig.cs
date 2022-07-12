namespace Common.Models.Auth;

public class LdapConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public string BaseContainer { get; set; } = string.Empty;
    public string[] SuperAdminLdapGroups { get; set; } = Array.Empty<string>();

    /// <summary>
    /// У LDAP сервера есть ограниечние на максимальное кол-во записей в ответе, обычно равно 1000. 
    /// Параметр введен для постраничного запроса.
    /// </summary>
    public int PageSize { get; set; }

    public Credentials ServiceUser { get; set; } = new Credentials();
}

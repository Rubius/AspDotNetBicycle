using Common.Models.Auth;
using LDAP.Extensions;
using Novell.Directory.Ldap;
using Serilog;
using System.Text;

namespace LDAP;

internal class LdapConnector
{
    private readonly LdapConfig _config;

    public int PageSize => _config.PageSize == 0 ? 1000 : _config.PageSize;
    public static string ServiceUserDn(string login, string domainName) => $"{login}@{domainName}";

    public LdapConnector(LdapConfig config)
    {
        _config = config;
    }

    public async Task<LdapConnection> CreateConnectionAndConnectAsync(string login, string password)
    {
        return await CreateConnectionAndConnectToLdapAsync(login, password);
    }

    public async Task<LdapConnection> CreateConnectionAndConnectAsync()
    {
        return await CreateConnectionAndConnectToLdapAsync(_config.ServiceUser.Login, _config.ServiceUser.Password);
    }

    private async Task<LdapConnection> CreateConnectionAndConnectToLdapAsync(
        string login,
        string password)
    {
        var connection = new LdapConnection
        {
            SecureSocketLayer = false,
            Constraints = new LdapSearchConstraints
            {
                ReferralFollowing = true,
                TimeLimit = 2 * 60 * 1000,
            }
        };

        try
        {
            await connection.ConnectAsync(_config.Host, _config.Port);
        }
        catch (LdapException e)
        {
            var errorMes = "Can't connect to LDAP";
            Log.Error($"{errorMes}\n Host: {_config.Host}\n Port: {_config.Port}\n Message: {e?.Message}");

            throw new LdapException(errorMes);
        }

        var serviceUserDn = ServiceUserDn(login, _config.DomainName);

        try
        {
            await connection.BindAsync(serviceUserDn, password);
        }
        catch (LdapException e)
        {
            var errorMessage = $"Invalid login/password for the service LDAP account '{serviceUserDn}'";
            Log.Error($"{errorMessage}\n Message: {e?.Message}");

            throw new LdapException(errorMessage);
        }

        if (!connection.Bound)
        {
            var errorMessage = $"Can't bind to LDAP with account '{serviceUserDn}'";
            Log.Error(errorMessage);
            throw new LdapException(errorMessage);
        }

        return connection;
    }

    public string GetSearchBase()
    {
        return _config.BaseContainer;
    }

    public string GetDomainName()
    {
        return GetSearchBase().DomainName();
    }
}

using Common.Models.Auth;
using Common.Services;
using LDAP.Constants;
using LDAP.Extensions;
using LDAP.Helpers;
using Localization.Resources;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using Serilog;

namespace LDAP;

public class LdapAuthenticator : IAuthenticatorService
{
    private readonly LdapConnector _connector;
    private readonly IPermissionService _permissionService;

    public LdapAuthenticator(
        IOptions<AuthSettings> authSettings,
        IPermissionService permissionService)
    {
        _connector = new LdapConnector(authSettings.Value.Ldap);
        _permissionService = permissionService;
    }

    private async Task<ExternalAuthUser?> AuthenticateAndGetUser(string login, string password)
    {
        Log.Information("AuthenticateAndGetUser");

        using var connection = await _connector.CreateConnectionAndConnectAsync(login, password);

        var searchBase = _connector.GetSearchBase();

        var searchFilter = LdapSearchStrBuilder.And(
            LdapSearchTerms.PersonCategory,
            LdapSearchTerms.UserClass,
            LdapSearchTerms.SamAccountName(login));

        Log.Information($"searchFilter = {searchFilter}");

        var result = await connection.SearchAsync(
            searchBase,
            LdapConnection.ScopeSub,
            searchFilter,
            new[] { LdapAttributes.Id, LdapAttributes.DisplayName, LdapAttributes.Mail, LdapAttributes.MemberOf },
            false
        );

        var user = result.Next();
        if (user == null)
        {
            return null;
        }

        var userId = new Guid(user.GetAttribute(LdapAttributes.Id).ByteValue);

        // email и displayName для пользователя необязательны. Но, к сожалению, функции проверки наличия
        // атрибута нет. Приходится ситуацию ловить через исключение
        string email;
        try
        {
            email = user.GetAttribute(LdapAttributes.Mail)?.StringValue ?? string.Empty;
        }
        catch
        {
            email = string.Empty;
        }

        var memberOf = user.MemberOf();

        var permissions = _permissionService.GetPermissionsFromLdapGroups(memberOf).ToList();
        if (!permissions.Any())
        {
            throw new Exception(Resources.UserIsNotInLdapSystemRoleGroup);
        }

        return new ExternalAuthUser(userId, LdapSecurityHelper.PrettifyDisplayName(login), email, permissions);
    }

    public async Task<ExternalAuthUser?> Authenticate(string login, string password)
    {
        try
        {
            return await AuthenticateAndGetUser(login, password);
        }
        catch (Exception ex)
        {
            Log.Information("Failed Ldap authentication routine for user\n Login: {Login}\n {$Exeption}", login, ex);
            return null;
        }
    }
}
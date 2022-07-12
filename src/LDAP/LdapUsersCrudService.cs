using Common.Models.Auth;
using Common.Models.Users;
using Common.Services;
using LDAP.Constants;
using LDAP.Extensions;
using LDAP.Helpers;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using Serilog;

namespace LDAP;

public class LdapUsersCrudService : IUsersCrudService
{
    private readonly IPermissionService _permissionService;
    private readonly LdapConnector _connector;
    private readonly ILogger _logger;

    public LdapUsersCrudService(
        IPermissionService permissionService,
        IOptions<AuthSettings> authSettings)
    {
        _permissionService = permissionService;
        var config = authSettings.Value.Ldap;
        _connector = new LdapConnector(config);
        _logger = Log.ForContext<LdapUsersCrudService>();
    }

    public async Task<User?> GetByIdAsync(Guid? userId)
    {
        if (userId == null)
            return null;

        var searchFilter = LdapSearchStrBuilder.And(LdapSearchTerms.PersonCategory,
                                                LdapSearchTerms.UserClass,
                                                LdapSearchTerms.ObjectGuid(userId.Value.ToLdapFilterString()));
        var users = await GetUsers(searchFilter);

        return users.FirstOrDefault();
    }

    public async Task<Dictionary<Guid, User>> GetByIdsAsync(IList<Guid> userGuids)
    {
        if (userGuids == null || userGuids.Count == 0)
        {
            return new Dictionary<Guid, User>();
        }

        var terms = userGuids.Select(x => LdapSearchTerms.ObjectGuid(x.ToLdapFilterString()));
        var searchFilter = LdapSearchStrBuilder.And(LdapSearchTerms.PersonCategory,
                                                LdapSearchTerms.UserClass,
                                                LdapSearchStrBuilder.Or(terms));
        var users = await GetUsers(searchFilter);

        return users.ToDictionary(x => x.Id);
    }

    public async Task<User[]> GetByAdGroups(IEnumerable<string> adGroups)
    {
        try
        {
            var adGroupsArray = adGroups.ToArray();
            if (!adGroupsArray.Any())
            {
                return Array.Empty<User>();
            }

            // DistinguishedName групп AD должны быть заэкранированы преде использованием в фильтре запроса
            var adGroupsDns = await GetGroupsDN(adGroups);
            var terms = adGroupsDns.Select(LdapSearchTerms.MemberOf);

            string searchFilter = LdapSearchStrBuilder.And(LdapSearchTerms.PersonCategory,
                                                            LdapSearchTerms.UserClass,
                                                            LdapSearchStrBuilder.Or(terms));

            return await GetUsers(searchFilter);
        }
        catch (LdapException ex)
        {
            Log.Error(ex, "GetByAdGroups Error");
            throw ex;
        }
    }

    /// <summary>
    /// Получить Dn по наименованиям групп AD, используемым в системе
    /// </summary>
    /// <param name="adGroups">Набор наименований групп AD <seealso cref="DistinguishedNameExtensions.FullGroupName(string)"/></param>
    /// <returns></returns>
    private Task<string[]> GetGroupsDN(IEnumerable<string> adGroups)
    {
        // TODO Требуется к реализации, если в системе не хранятся DistinguishedName для групп AD
        return Task.FromResult(adGroups.Select(DistinguishedNameExtensions.EscapeForSearch).ToArray());
    }

    /// <summary>
    /// Получить фильтрованный набор пользовалей
    /// </summary>
    /// <param name="searchFilter">Фильтр поиска</param>
    /// <returns>Массив пользователей, удовлетворяющих переданному фильтру</returns>
    private async Task<User[]> GetUsers(string searchFilter)
    {
        try
        {
            using var connection = await _connector.CreateConnectionAndConnectAsync();

            var searchResults = await FindAsync(connection, searchFilter);

            var users = new List<User>(searchResults.Count);
            foreach (var entry in searchResults)
            {
                try
                {
                    var permissions = _permissionService.GetPermissionsFromLdapGroups(entry.MemberOf());
                    var user = GetUser(entry, permissions);
                    users.Add(user);
                }
                catch (Exception ex)
                {
                    Log.Warning("Error while getting user internal data: {message} {ex}", ex.Message, ex);
                }
            }

            return users.ToArray();
        }
        catch (LdapException ex)
        {
            Log.Error(ex, "GetUsers Error");
            return Array.Empty<User>();
        }
    }

    public async Task<IEnumerable<string>> GetAdGroups()
    {
        try
        {
            using var connection = await _connector.CreateConnectionAndConnectAsync();
            string searchFilter = LdapSearchStrBuilder.And(LdapSearchTerms.GroupCategory,
                                                            LdapSearchTerms.ObjectClass("*"));

            var searchResults = await FindAsync(connection, searchFilter);

            var result = new List<string>();
            foreach (var entry in searchResults)
            {
                try
                {
                    result.Add(entry.FullGroupName());
                }
                catch (Exception ex)
                {
                    Log.Warning("GetAdGroups exception: {message} \n {exception}", ex.Message, ex);
                }
            }

            return result;
        }
        catch (LdapException ex)
        {
            Log.Error(ex, "Error while getting AD groups list");
            return Enumerable.Empty<string>();
        }
    }

    public async Task CheckAllUsersExistAsync(IEnumerable<string> userAccountNames)
    {
        var searchFilter = (string sAmAccounts) => LdapSearchStrBuilder.And(
            LdapSearchTerms.PersonCategory,
            LdapSearchTerms.UserClass,
            sAmAccounts);
        var getEntryName = (LdapEntry entry) => GetUser(entry, new List<Permission>()).Login;

        await CheckAllExistAsync(userAccountNames, searchFilter, getEntryName);
    }

    public async Task CheckAllGroupsExistAsync(IEnumerable<string> ldapGroups)
    {
        var searchFilter = (string sAmAccounts) => LdapSearchStrBuilder.And(
            LdapSearchTerms.GroupCategory,
            sAmAccounts);

        await CheckAllExistAsync(ldapGroups, searchFilter, (entry) => entry.FullGroupName());
    }

    private async Task CheckAllExistAsync(IEnumerable<string> items, Func<string, string> buildFilter, Func<LdapEntry, string> getItemName)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "Empty list");
        }

        var itemsSet = items.ToHashSet();
        var accountNames = itemsSet.Select(LdapSearchTerms.SamAccountName);
        var searchFilter = buildFilter(LdapSearchStrBuilder.Or(accountNames));
        List<LdapEntry> searchResult = new();
        try
        {
            using var connection = await _connector.CreateConnectionAndConnectAsync();
            searchResult = await FindAsync(connection, searchFilter);
        }
        catch (LdapException exception)
        {
            _logger.Warning($"Ldap groups error\n{exception}");
        }

        var ldapFoundItems = searchResult.Select(getItemName);
        foreach (var item in itemsSet)
        {
            if (!ldapFoundItems.Contains(item))
            {
                throw new ArgumentException($"Can't find ldap entry with sAMAccountname '{item}'");
            }
        }
    }

    private async Task<List<LdapEntry>> FindAsync(ILdapConnection connection, string searchFilter)
    {
        try
        {
            var searchBase = _connector.GetSearchBase();
            var constr = new LdapConstraints();
            var searchConstr = new LdapSearchConstraints(constr)
            {
                MaxResults = int.MaxValue,
            };

            SearchOptions opt = new(searchBase, LdapConnection.ScopeSub, searchFilter, new[]
            {
                    LdapAttributes.Id,
                    LdapAttributes.SamAccountName,
                    LdapAttributes.DisplayName,
                    LdapAttributes.MemberOf
                }, false, searchConstr);
            var result = await connection.SearchUsingSimplePagingAsync(opt, _connector.PageSize) ?? new List<LdapEntry>();

            Log.Information("SEARCH RESULT COUNT: {count}; Filter: {filter}", result.Count, searchFilter);

            return result;
        }
        catch (Exception exception)
        {
            _logger.Warning(@"LDAP: Cannot search
            Search Filter: {@searchFilter}
            {@exception}", searchFilter, exception);

            throw;
        }
    }

    private static User GetUser(LdapEntry entry, IEnumerable<Permission> permissions)
        => new()
        {
            Id = new Guid(entry.GetAttribute(LdapAttributes.Id).ByteValue),
            Login = entry.GetAttribute(LdapAttributes.SamAccountName).StringValue,
            Name = LdapSecurityHelper.PrettifyDisplayName(entry.GetAttribute(LdapAttributes.SamAccountName).StringValue),
            Permissions = permissions,
        };
}

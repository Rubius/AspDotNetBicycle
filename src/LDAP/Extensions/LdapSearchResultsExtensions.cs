using Novell.Directory.Ldap;
using Serilog;

namespace LDAP.Extensions;

public static class LdapSearchResultsExtensions
{
    public static IEnumerable<LdapEntry> MembersOfGroup(this ILdapSearchResults searchResults, string group)
    {
        var result = new List<LdapEntry>();

        while (searchResults.HasMore())
        {
            try
            {
                var entry = searchResults.Next();

                if (entry.MemberOf().Contains(group))
                {
                    result.Add(entry);
                }
            }
            catch (LdapException ex)
            {
                Log.Information(ex, "LDAP MembersOfGroup exception");
            }
        }

        return result;
    }

    public static IEnumerable<LdapEntry> MembersOfGroups(this ILdapSearchResults searchResults, IEnumerable<string> groups)
    {
        var result = new List<LdapEntry>();

        while (searchResults.HasMore())
        {
            try
            {
                var entry = searchResults.Next();

                if (entry.MemberOf().Intersect(groups).Any())
                {
                    result.Add(entry);
                }
            }
            catch (LdapException ex)
            {
                Log.Error(ex, "LDAP MembersOfGroups exception!");
            }
        }

        return result;
    }
}
namespace LDAP.Extensions;

public static class GuidExtensions
{
    public static string ToLdapFilterString(this Guid guid)
        => string.Join("", guid.ToByteArray().Select(x => $@"\{x:x2}"));
}
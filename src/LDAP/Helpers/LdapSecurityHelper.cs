namespace LDAP.Helpers;

public static class LdapSecurityHelper
{
    public static string PrettifyDisplayName(string srcDisplayName)
    {
        var result = srcDisplayName.Replace('.', ' ');
        return result;
    }
}
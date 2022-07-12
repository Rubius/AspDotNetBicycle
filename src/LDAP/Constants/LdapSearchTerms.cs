namespace LDAP.Constants;

/// <summary>
/// Фильтры строки поиска в запросах к AD
/// </summary>
public class LdapSearchTerms
{
    public static string ObjectCategory(string objCategory) => Term(LdapAttributes.ObjectCategory, objCategory);
    public static string PersonCategory => ObjectCategory("person");
    public static string GroupCategory => ObjectCategory("group");

    public static string ObjectClass(string objClass) => Term(LdapAttributes.ObjectClass, objClass);
    public static string UserClass => ObjectClass("user");

    public static string SamAccountName(string accountName) => Term(LdapAttributes.SamAccountName, accountName);

    public static string ObjectGuid(string guid) => Term(LdapAttributes.Id, guid);

    public static string Cn(string cn) => Term(LdapAttributes.CN, cn);

    public static string MemberOf(string memberOf) => Term(LdapAttributes.MemberOf, memberOf);


    private static string Term(string attributeName, string attributeValue)
    {
        if (string.IsNullOrEmpty(attributeValue))
        {
            return string.Empty;
        }

        return $"({attributeName}={attributeValue})";
    }
}

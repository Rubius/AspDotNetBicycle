using LDAP.Constants;
using Novell.Directory.Ldap;

namespace LDAP.Extensions;

public static class LdapEntryExtensions
{
    /// <summary>
    /// Наименования групп AD, к которым принадлежит сущность
    /// </summary>
    /// <param name="entry">Сущность AD</param>
    /// <returns>Список наименований</returns>
    public static IEnumerable<string> MemberOf(this LdapEntry entry)
    {
        try
        {
            return entry.GetAttribute(LdapAttributes.MemberOf).StringValueArray
                .Select(value => value.FullGroupName());
        }
        catch (KeyNotFoundException)
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Домен, в котором определена сущность
    /// </summary>
    /// <param name="entry">Сущность AD</param>
    /// <returns>Наименование домена</returns>
    public static string DomainName(this LdapEntry entry)
    {
        return entry.Dn.DomainName();
    }

    /// <summary>
    /// Получить преобразованное наименование сущности AD по DistinguishedName
    /// </summary>
    /// <param name="entry">Сущность AD</param>
    /// <returns>Преобразованное наименование сущности</returns>
    public static string FullGroupName(this LdapEntry entry)
    {
        return entry.Dn.FullGroupName();
    }

    /// <summary>
    /// Экранировать DistinguishedName сущности 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns>Экранированное представление DN</returns>
    public static string EscapedDn(this LdapEntry entry)
    {
        return entry.Dn.EscapeForSearch();
    }
}
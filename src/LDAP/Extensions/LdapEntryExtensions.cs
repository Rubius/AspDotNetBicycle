using LDAP.Constants;
using Novell.Directory.Ldap;

namespace LDAP.Extensions;

public static class LdapEntryExtensions
{
    /// <summary>
    /// ������������ ����� AD, � ������� ����������� ��������
    /// </summary>
    /// <param name="entry">�������� AD</param>
    /// <returns>������ ������������</returns>
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
    /// �����, � ������� ���������� ��������
    /// </summary>
    /// <param name="entry">�������� AD</param>
    /// <returns>������������ ������</returns>
    public static string DomainName(this LdapEntry entry)
    {
        return entry.Dn.DomainName();
    }

    /// <summary>
    /// �������� ��������������� ������������ �������� AD �� DistinguishedName
    /// </summary>
    /// <param name="entry">�������� AD</param>
    /// <returns>��������������� ������������ ��������</returns>
    public static string FullGroupName(this LdapEntry entry)
    {
        return entry.Dn.FullGroupName();
    }

    /// <summary>
    /// ������������ DistinguishedName �������� 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns>�������������� ������������� DN</returns>
    public static string EscapedDn(this LdapEntry entry)
    {
        return entry.Dn.EscapeForSearch();
    }
}
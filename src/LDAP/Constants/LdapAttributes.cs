namespace LDAP.Constants
{
    /// <summary>
    /// Аттрибуты сущностей AD
    /// </summary>
    public static class LdapAttributes
    {
        public const string Id = "objectGUID";
        public const string SamAccountName = "sAMAccountName";
        public const string MemberOf = "memberOf";
        public const string DisplayName = "displayName";
        public const string Mail = "mail";

        public const string CN = "CN";
        public const string ObjectClass = "objectClass";
        public const string ObjectCategory = "objectCategory";
    }
}

using Novell.Directory.Ldap;
using Serilog;
using System.Text;

namespace LDAP.Extensions
{
    internal static class DistinguishedNameExtensions
    {
        // Длина наименования аттрибута, указанного в DistinguishedName сущности (со знаком равно включительно) (пример наименования аттрибута 'CN=')
        private const int AttributeWithEqualsLength = 3;

        /// <summary>
        /// Получить домен из DistinguishedName сущности
        /// </summary>
        /// <param name="dn">DistinguishedName сущности</param>
        /// <returns>Домен</returns>
        public static string DomainName(this string dn)
        {
            if (string.IsNullOrEmpty(dn))
                return string.Empty;
            var attributes = GetAttributes(dn);
            var domainParts = attributes.Where(y => y.StartsWith("DC="));
            var domainValues = GetAttributesValues(domainParts);

            return string.Join('.', domainValues).ToUpper();
        }

        private static string[] GetAttributes(string dn)
        {
            var result = Array.Empty<string>();

            try
            {
                result = LdapDn.ExplodeDn(dn, false);
            }
            catch (ArgumentException)
            {
                Log.Warning(@"Can`t get DN attributes! Incorrect dn: {dn}", dn);
            }

            return result ?? Array.Empty<string>();
        }

        /// <summary>
        /// Получить значения аттрибутов из DistinguishedName
        /// </summary>
        /// <param name="attributes">Набор аттрибутов</param>
        /// <returns>Строковые значения аттрибутов</returns>
        private static IEnumerable<string> GetAttributesValues(IEnumerable<string> attributes)
        {
            var values = new List<string>(attributes.Count());
            foreach (var attr in attributes)
            {
                if (attr.Length > AttributeWithEqualsLength)
                {
                    values.Add(attr[AttributeWithEqualsLength..]);
                }
            }

            return values;
        }

        /// <summary>
        /// Получить преобразованное наименование группы AD
        /// </summary>
        /// <param name="dn">DistinguishedName группы</param>
        /// <returns>Преобразованное наименование группы AD</returns>
        public static string FullGroupName(this string dn)
        {
            // TODO Требуется к реализации, если в системе требуется преобразованное наименование группы AD
            return dn ?? string.Empty;
        }

        /// <summary>
        /// Получить DistinguishedName с экранированными спец-символами
        /// </summary>
        /// <param name="dn">DistinguishedName сущности</param>
        /// <returns>DistinguishedName с экранированными символами</returns>
        public static string EscapeForSearch(this string dn)
        {
            if (string.IsNullOrEmpty(dn))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(dn);
            var rules = new Dictionary<char, string>
            {
                { '\\', "\\5C" },
                { '*', "\\2A" },
                { '(', "\\28" },
                { ')', "\\29" },
            };
            var nullStrToReplace = "\\00";

            for (int i = 0; i < sb.Length; i++)
            {
                var symbol = sb[i];
                if (rules.ContainsKey(symbol))
                {
                    i = ReplaceSpecialSymbol(sb, rules[symbol], i);
                }
                else if ((byte)symbol == 0)
                {
                    i = ReplaceSpecialSymbol(sb, nullStrToReplace, i);
                }
            }
            var result = sb.ToString();
            return result;
        }

        private static int ReplaceSpecialSymbol(StringBuilder sb, string str, int i)
        {
            sb.Remove(i, 1);
            sb.Insert(i, str);
            i = i + str.Length - 1;

            return i;
        }
    }
}

using System.Text;

namespace LDAP.Helpers;

/// <summary>
/// Построитель фильтра для запросов к AD
/// </summary>
public class LdapSearchStrBuilder
{
    /// <summary>
    /// Установить логическое 'И' для набора условий
    /// </summary>
    /// <param name="terms">Набор условий над аттрибутами AD</param>
    /// <returns>Результирующая строка с объединенными условиями</returns>
    public static string And(params string[] terms)
    {
        return Build('&', terms);
    }

    /// <summary>
    /// Установить логическое 'ИЛИ' для набора условий
    /// </summary>
    /// <param name="terms">Набор условий над аттрибутами AD</param>
    /// <returns>Результирующая строка с объединенными условиями</returns>
    public static string Or(params string[] terms)
    {
        return Or(terms.AsEnumerable());
    }

    /// <summary>
    /// Установить логическое 'ИЛИ' для набора условий
    /// </summary>
    /// <param name="terms">Набор условий над аттрибутами AD</param>
    /// <returns>Результирующая строка с объединенными условиями</returns>
    public static string Or(IEnumerable<string> terms)
    {
        return Build('|', terms);
    }

    /// <summary>
    /// Установить логическое 'НЕ' для условия
    /// </summary>
    /// <param name="terms">Условие над аттрибутом AD</param>
    /// <returns>Результирующая строка с условием</returns>
    public static string Not(string term)
    {
        return Build('!', new[] { term });
    }

    private static string Build(char boolSymbol, IEnumerable<string> terms)
    {
        if (!terms.Any())
        {
            return string.Empty;
        }

        string startedStr = "(" + boolSymbol;
        StringBuilder sb = new(startedStr);
        terms = terms.Where(x => !string.IsNullOrEmpty(x));

        foreach (var term in terms)
        {
            sb.Append(term);
        }
        sb.Append(')');

        return sb.ToString();
    }
}

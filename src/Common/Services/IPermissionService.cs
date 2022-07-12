using Common.Models.Users;

namespace Common.Services;
/// <summary>
/// Сервис получения типов доступа пользователя по данным пользователя
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Получить типы доступа пользователя по группам в AD
    /// </summary>
    /// <param name="memberOf">Список наименований групп AD, в которых состоит пользователь</param>
    /// <returns></returns>
    IEnumerable<Permission> GetPermissionsFromLdapGroups(IEnumerable<string> memberOf);
}
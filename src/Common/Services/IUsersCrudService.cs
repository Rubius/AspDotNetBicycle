using Common.Models.Users;

namespace Common.Services;

/// <summary>
/// Сервис для операций над пользователями
/// </summary>
public interface IUsersCrudService
{
    /// <summary>
    /// Получить пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Данные о пользователе</returns>
    Task<User?> GetByIdAsync(Guid? userId);

    /// <summary>
    /// Получить пользователей по идентификаторам
    /// </summary>
    /// <param name="userGuids">Список идентификаторов</param>
    /// <returns>Словарь пользователей с ключом по идентификатору</returns>
    Task<Dictionary<Guid, User>> GetByIdsAsync(IList<Guid> userGuids);

    /// <summary>
    /// Получить пользователей по группам AD
    /// </summary>
    /// <param name="adGroups">Список групп AD</param>
    /// <returns>Пользователи, которые входят хотя бы в одну из указанных групп</returns>
    Task<User[]> GetByAdGroups(IEnumerable<string> adGroups);

    /// <summary>
    /// Получить список наименований имеющихся групп AD
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<string>> GetAdGroups();

    /// <summary>
    /// Проверить, что все группы существуют в AD
    /// </summary>
    /// <param name="ldapGroups"></param>
    /// <returns></returns>
    Task CheckAllGroupsExistAsync(IEnumerable<string> ldapGroups);

    /// <summary>
    /// Проверить, что все пользователи существуют В AD
    /// </summary>
    /// <param name="userLogins">Логины пользователей</param>
    /// <returns></returns>
    Task CheckAllUsersExistAsync(IEnumerable<string> userLogins);
}
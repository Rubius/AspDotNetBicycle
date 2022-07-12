using Common.Models.Users;

namespace Common.Services;

/// <summary>
/// Получение данных пользователя, инициировавшего запрос
/// </summary>
public interface ICurrentUserService
{
    public User? User { get; }  
}
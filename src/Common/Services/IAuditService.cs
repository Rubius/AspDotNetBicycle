namespace Common.Services;

/// <summary>
/// Сервис логирования действий пользователя
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Залогировать действие пользователя
    /// </summary>
    /// <param name="action">Название действия</param>
    /// <param name="comment">Комментарий</param>
    /// <param name="metadata">Дополнительные данные</param>
    void LogAudit(string action, string comment, object? metadata = null);
}
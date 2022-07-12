namespace Domain.Exceptions;

/// <summary>
/// Ошибка инициализации сущности
/// </summary>
public class MappingValidationException : Exception
{
    public MappingValidationException(string message, string propertyName, string typeName) : base(message)
    {
        PropertyName = propertyName;
        TypeName = typeName;
    }

    public string PropertyName { get; }
    public string TypeName { get; }
}

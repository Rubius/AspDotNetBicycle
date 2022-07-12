namespace Application.Common.Exceptions.CustomValidationException;

public class ObjectValidationError
{
    public Dictionary<string, string> ValidationErrors { get; set; } = new();
}
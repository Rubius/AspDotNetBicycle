namespace Application.Common.Exceptions.CustomValidationException;

public class ObjectsListValidationErrorItem
{
    public ObjectsListValidationErrorItem(string propertyName, string messageText)
    {
        PropertyName = propertyName;
        MessageText = messageText;
    }

    public string PropertyName { get; }

    public string MessageText { get; }
}

public class ObjectsListValidationError
{
    public Dictionary<string, Dictionary<string, string>> ListObjectsValidationError { get; }

    public ObjectsListValidationError(Dictionary<int, List<ObjectsListValidationErrorItem>> errors)
    {
        ListObjectsValidationError = errors.ToDictionary(
            x => x.Key.ToString(),
            y => y.Value.ToDictionary(xx => char.ToLowerInvariant(xx.PropertyName[0]) + xx.PropertyName[1..], yy => yy.MessageText));
    }
}

public class ObjectsListValidationException : Exception
{
    public Dictionary<int, List<ObjectsListValidationErrorItem>> Errors { get; }

    public ObjectsListValidationException(Dictionary<int, List<ObjectsListValidationErrorItem>> errors)
    {
        Errors = errors;
    }
}
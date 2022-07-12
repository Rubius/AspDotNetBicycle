using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApp.Middlewares.ExceptionHandling;

public class AnnotatedProblemDetails
{
    public AnnotatedProblemDetails(ProblemDetails problemDetails)
    {
        Detail = problemDetails.Detail;
        Instance = problemDetails.Instance;
        Status = problemDetails.Status;
        Title = problemDetails.Title;
        Type = problemDetails.Type;

        foreach (var kvp in problemDetails.Extensions)
        {
            Extensions[kvp.Key] = kvp.Value;
        }
    }

    [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }

    [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
    public int? Status { get; set; }

    [JsonProperty(PropertyName = "detail", NullValueHandling = NullValueHandling.Ignore)]
    public string? Detail { get; set; }

    [JsonProperty(PropertyName = "instance", NullValueHandling = NullValueHandling.Ignore)]
    public string? Instance { get; set; }

    [JsonProperty(PropertyName = "extensions", NullValueHandling = NullValueHandling.Ignore)]
    public IDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);
}
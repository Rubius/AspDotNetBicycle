using Application.Common.Exceptions;
using Application.Common.Exceptions.CustomValidationException;
using Application.Common.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Diagnostics;
using System.Net;
using Localization.Resources;

namespace WebApp.Middlewares.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
    private const string JsonResponseContentType = "application/json";
    private const int MaxAvailableRequestTime = 3000;

    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _configuration;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration)
    {
        _next = next;
        _webHostEnvironment = webHostEnvironment;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            await _next(context);
        }
        catch (AbstractHttpException exception)
        {
            await HandleHttpException(exception, context);
        }
        catch (NotSupportedException exception)
        {
            await HandleHttpException(new NotAllowedException(exception.Message), context);
        }
        catch (ValidationException exception)
        {
            await HandleValidationException(exception, context);
        }
        catch (ObjectsListValidationException exception)
        {
            await HandleListObjectsValidationException(exception, context);
        }
        catch (Exception exception)
        {
            await HandleUnknownException(exception, context);
        }
        finally
        {
            stopWatch.Stop();
            if (stopWatch.ElapsedMilliseconds >= MaxAvailableRequestTime)
            {
                var requestInfo = string.Join("  ", context.Request.RouteValues.Select(x => $"Key: {x.Key}; Value: {x.Value},").ToList());
                Log.Warning($"Too long request. Request Info: ({requestInfo})\n" +
                            $"Duration: {GetSwMinutes(stopWatch)}");
            }
        }
    }
    private static string GetSwMinutes(Stopwatch stopwatch)
    {
        var ts = stopwatch.Elapsed;

        return $"{(int)ts.TotalMinutes} min. {ts.Seconds} sec. {ts.Milliseconds} ms.";
    }

    private async Task HandleHttpException(AbstractHttpException exception, HttpContext context)
    {
        context.Response.StatusCode = exception.StatusCode;
        context.Response.ContentType = JsonResponseContentType;

        var error = exception.GetApiError(_webHostEnvironment.IsDevelopment());

        await context.Response.WriteAsync(SerializeApiError(error));

        Log.Error("Http Exception\n" +
                  $"{exception}");
    }

    private static async Task HandleValidationException(ValidationException exception, HttpContext context)
    {
        const int statusCode = (int)HttpStatusCode.BadRequest;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = JsonResponseContentType;

        var error = new ApiError(statusCode, Resources.DataValidationError);

        var validationError = new ObjectValidationError()
        {
            ValidationErrors = exception.Errors.ToDictionary(x => char.ToLowerInvariant(x.PropertyName[0]) + x.PropertyName[1..]
                , x => x.ErrorMessage),
        };
        error.Extensions = validationError;

        await context.Response.WriteAsync(SerializeApiError(error));

        Log.Error("Validation Exception\n" +
                  $"{exception}");
    }

    private static async Task HandleListObjectsValidationException(ObjectsListValidationException exception, HttpContext context)
    {
        const int statusCode = (int)HttpStatusCode.BadRequest;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = JsonResponseContentType;

        var error = new ApiError(statusCode, Resources.DataValidationError)
        {
            Extensions = new ObjectsListValidationError(exception.Errors)
        };

        await context.Response.WriteAsync(SerializeApiError(error));

        Log.Error("List Objects Validation Exception\n" +
                  $"{exception}");
    }

    private static string SerializeApiError(ApiError error)
    {
        return JsonConvert.SerializeObject(error, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        });
    }

    private async Task HandleUnknownException(Exception exception, HttpContext context)
    {
        Log.Error("Unknown exception\n" +
                          $"{exception}");

        var details = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = Resources.UnhandledApplicationError,
            Detail = _webHostEnvironment.IsDevelopment() || _configuration.IsTestEnvironment()
                ? exception.ToString()
                : Resources.InternalServerError,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = details.Status!.Value;
        context.Response.ContentType = JsonResponseContentType;

        var annotatedDetails = new AnnotatedProblemDetails(details);
        var detailsJson = JsonConvert.SerializeObject(annotatedDetails);
        await context.Response.WriteAsync(detailsJson);

        Log.Error("Internal Exception\n" +
                  $"{exception}");
    }
}
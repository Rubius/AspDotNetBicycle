using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace WebApp.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddOpenApiDocument(this IServiceCollection services)
    {
        services.AddOpenApiDocument(settings =>
        {
            settings.Title = "API";
            settings.DocumentName = "v1";
            settings.SchemaType = SchemaType.OpenApi3;
            settings.GenerateEnumMappingDescription = true;

            settings.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Cookie,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });
            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

            settings.PostProcess = document =>
            {
                document.Schemes = new[] { OpenApiSchema.Https, OpenApiSchema.Http };
            };

        });

        return services;

    }
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        app.UseOpenApi(settings =>
        {
            settings.Path = "/api/swagger/{documentName}/swagger.json";
            settings.PostProcess = (document, _) =>
            {
                document.Schemes = new[] { OpenApiSchema.Https, OpenApiSchema.Http };
            };
        });

        app.UseSwaggerUi3(settings =>
        {
            settings.Path = "/api/swagger";
            settings.DocumentPath = "/api/swagger/{documentName}/swagger.json";
        });

        return app;
    }
}
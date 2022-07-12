using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Localization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using WebApp.Extensions;
using WebApp.Middlewares.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

var generationBuild = args.Length >= 1;
if (generationBuild && args[0].Contains("appsettings.json"))
{
    builder.Configuration.AddJsonFile(args[0], optional: false);
}

SetupLog(builder.Configuration);

builder.Services.AddControllers();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

RegisterCors(builder);

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddJsonOptions()
    .AddOpenApiDocument();

builder.Services
    .AddApplication()
    .AddAppAuthentication(builder.Configuration, true)
    .AddAppLocalization()
    .AddInfrastructure(builder.Configuration, isGenerationBuild: generationBuild);

builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

app.UseForwardedHeaders();

await MigrateDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

// TODO: включить
//app.UseRequestFilter();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("CorsPolicy")
   .UseRouting()
   .UseHttpsRedirection()
   .UseAppAuthentication();

app.MapControllers();

app.UseHealthAndLifeCheck();

app.Run();

void SetupLog(ConfigurationManager configuration)
{
    var loggingSection = configuration.GetSection("Logging");
    var logLevelSection = loggingSection.GetSection("LogLevel");
    var logLevelString = logLevelSection["Default"];

    var couldParse = Enum.TryParse<LogEventLevel>(logLevelString, out var logLevel);
    if (!couldParse)
    {
        logLevel = LogEventLevel.Information;
    }

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", logLevel)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    if (!couldParse)
    {
        Log.Warning("Couldn't get the log level from the configuration");
    }
}

void RegisterCors(WebApplicationBuilder webApplicationBuilder)
{
    var corsOrigin = webApplicationBuilder.Configuration["CorsOrigin"];
    if (!string.IsNullOrEmpty(corsOrigin))
    {
        webApplicationBuilder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins(corsOrigin)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
        });
    }
}

async Task MigrateDatabase(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (context == null)
        throw new Exception("Cannot initialize the database context");

    await context.Database.MigrateAsync();
}

public partial class Program { }
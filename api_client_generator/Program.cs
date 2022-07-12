using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using SwaggerGen;
using System.Diagnostics;

var cSharpClientOutputDirectory = args[0];
var typeScriptClientOutputDirectory = args[1];
var swaggerJsonUrl = args[2];

OpenApiDocument? openApiDocument = await TryCreateOpenApiDocumentAsync();
if (openApiDocument is null)
{
    Console.WriteLine("Не удалось сформировать swagger.json");
    return;
}

var csharpClientPath = Path.Combine(cSharpClientOutputDirectory, "ApiClient.g.cs");
var typescriptClientPath = Path.Combine(typeScriptClientOutputDirectory, "ApiClient.ts");

await Task.WhenAll(
    GenerateCsharpClient(),
    GenerateTypeScriptClient()
);

async Task<OpenApiDocument?> TryCreateOpenApiDocumentAsync()
{
    OpenApiDocument? document = null;
    using var process = new Process();
    try
    {
        var webAppDirectory = AssembliesHelper.GetWebAppAssemblyBinDirectory();
        var appsettingsJsonPath = Path.Combine(webAppDirectory, "appsettings.json");

        process.StartInfo.FileName = Path.Combine(webAppDirectory, "WebApp.exe");
        process.StartInfo.Arguments = appsettingsJsonPath;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.UseShellExecute = false;

        process.Start();

        HttpClient client = new();
        int requestsCount = 0;
        var hasConnection = false;
        while (requestsCount < 5 && !hasConnection)
        {
            try
            {
                Console.WriteLine(swaggerJsonUrl);
                var resp = await client.GetAsync(swaggerJsonUrl);
                hasConnection = resp.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request № {requestsCount}");
                Console.Error.WriteLine(ex.Message);
            }
            requestsCount++;
            Thread.Sleep(1000);
        }

        if (hasConnection)
        {
            document = await OpenApiDocument.FromUrlAsync(swaggerJsonUrl);
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        Console.WriteLine("Нажмите любую клавишу для завершения");
        Console.ReadKey();
    }
    finally
    {
        process.Close();
    }

    return document;
}

async Task GenerateCsharpClient()
{
    var settings = new CSharpClientGeneratorSettings
    {
        ClassName = "{controller}ApiClient",
        GenerateDtoTypes = false,
        GenerateClientClasses = true,
        GenerateOptionalParameters = true,
        CSharpGeneratorSettings =
        {
            Namespace = "IntegrationTests",
            TypeNameGenerator = new TypeNameWithNameSpaceGenerator()
        }
    };

    var generator = new CSharpClientGenerator(openApiDocument, settings);
    var generatedFile = generator.GenerateFile();
    await File.WriteAllTextAsync(csharpClientPath, generatedFile);
}

async Task GenerateTypeScriptClient()
{
    var settings = new TypeScriptClientGeneratorSettings
    {
        ClassName = "{controller}ApiClient",
        Template = TypeScriptTemplate.Axios,
        GenerateDtoTypes = true,
        GenerateClientClasses = true,
        GenerateOptionalParameters = true,
        UseTransformOptionsMethod = true,
        ClientBaseClass = "BaseApiClient",
        TypeScriptGeneratorSettings =
        {
            TypeScriptVersion = 4.0m,
            ExtensionCode = "import { BaseApiClient } from './BaseApiClient';"
        }
    };
    var generator = new TypeScriptClientGenerator(openApiDocument, settings);
    var generatedFile = generator.GenerateFile();
    await File.WriteAllTextAsync(typescriptClientPath, generatedFile);
}

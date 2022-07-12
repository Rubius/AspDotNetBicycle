using System.Reflection;
using WebApp.Helpers;

namespace SwaggerGen;

public static class AssembliesHelper
{
    public static Assembly GetWebAppAssembly()
    {
        return Assembly.GetAssembly(typeof(WebAppBeacon))!;
    }

    public static string GetWebAppAssemblyBinDirectory()
    {
        var result = Path.GetDirectoryName(GetWebAppAssembly().Location)!;
        return result;
    }
}
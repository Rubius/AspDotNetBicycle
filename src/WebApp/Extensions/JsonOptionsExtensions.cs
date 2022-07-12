using System.Text.Json.Serialization;

namespace WebApp.Extensions
{
    public static class JsonOptionsExtensions
    {
        public static IServiceCollection AddJsonOptions(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return services;
        }
    }
}

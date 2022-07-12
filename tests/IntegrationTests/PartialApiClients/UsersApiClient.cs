using Common.Extensions;
using System.Linq;
using System.Net.Http;

// Должен совпадать с namespace сгенерированного клинта
namespace IntegrationTests
{
    public partial class UsersApiClient
    {
        partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
            if (response.Headers.All(x => x.Key != "Set-Cookie"))
                return;
            
            var cookie = response.Headers
                .FirstOrDefault(x => x.Key == "Set-Cookie")
                .Value?
                .FirstOrDefault(x => x.Contains(HttpContextExtensions.JWT_COOKIE_KEY));

            if (cookie is null) 
                return;

            if (cookie.Contains($"{HttpContextExtensions.JWT_COOKIE_KEY}=;"))
            {
                client.DefaultRequestHeaders.Remove("Cookie");
            }
            else
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
            }
        }
    }
}

using Swashbuckle.Swagger;

namespace Collector.Common.Swagger.Extensions.Security
{
    public class BearerTokenScheme : SecurityScheme
    {
        public BearerTokenScheme()
        {
            description = "Bearer Token";
            type = "apiKey";
            vendorExtensions.Add("in", "header");
            vendorExtensions.Add("name", "Authorization");
        }
    }
}
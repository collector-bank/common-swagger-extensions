using Swashbuckle.Swagger;

namespace Collector.Common.Swagger.Extensions.Security
{
    public class BaererTokenScheme : SecurityScheme
    {
        public BaererTokenScheme()
        {
            description = "Baerer Token";
            type = "apiKey";
            vendorExtensions.Add("in", "header");
            vendorExtensions.Add("name", "Authorization");
        }
    }
}
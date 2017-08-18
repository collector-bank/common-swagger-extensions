using Swashbuckle.AspNetCore.Swagger;

namespace Collector.Common.Swagger.AspNetCore.Extensions.Security
{
    public class BearerTokenScheme : SecurityScheme
    {
        public BearerTokenScheme()
        {
            Description = "Bearer Token";
            Type = "apiKey";
            Extensions.Add("in", "header");
            Extensions.Add("name", "Authorization");
        }
    }
}
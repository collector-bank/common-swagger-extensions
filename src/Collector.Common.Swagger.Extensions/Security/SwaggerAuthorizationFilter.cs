using System;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Collector.Common.Swagger.Extensions.Security
{
    public partial class SwaggerAuthorizationFilter : IDocumentFilter
    {
        public static SwaggerAuthorizationFilter CreateWithAuthorizationFilter(Func<ApiDescription, bool> apiFilter) =>
            new SwaggerAuthorizationFilter(apiFilter);

        private readonly Func<ApiDescription, bool> _showAction;

        private SwaggerAuthorizationFilter(Func<ApiDescription, bool> apiFilter) => _showAction = apiFilter;

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var descriptions = apiExplorer.ApiDescriptions;
            //apiExplorer.ApiDescriptions[0].
            foreach (var description in descriptions)
            {
                // check if this action should be visible
                var notShowen = _showAction?.Invoke(description) ?? false;

                if (!notShowen)
                    continue; // user passed all permissions checks

                var route = "/" + description.Route.RouteTemplate.TrimEnd('/');
                var path = swaggerDoc.paths[route];

                // remove method or entire path (if there are no more methods in this path)
                switch (description.HttpMethod.Method)
                {
                    case "DELETE": path.delete = null; break;
                    case "GET": path.get = null; break;
                    case "HEAD": path.head = null; break;
                    case "OPTIONS": path.options = null; break;
                    case "PATCH": path.patch = null; break;
                    case "POST": path.post = null; break;
                    case "PUT": path.put = null; break;
                    default: throw new ArgumentOutOfRangeException("Method name not mapped to operation");
                }

                if (path.delete == null && path.get == null &&
                    path.head == null && path.options == null &&
                    path.patch == null && path.post == null && path.put == null)
                    swaggerDoc.paths.Remove(route);
            }
        }
    }
}
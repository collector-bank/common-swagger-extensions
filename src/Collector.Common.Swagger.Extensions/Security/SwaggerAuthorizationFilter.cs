using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Collector.Common.Swagger.Extensions.Security
{
    public class SwaggerAuthorizationFilter : IDocumentFilter
    {
        public static SwaggerAuthorizationFilter CreateWithAuthorizationFilter(Func<ApiDescription, bool> apiFilter) =>
            new SwaggerAuthorizationFilter(apiFilter);

        private readonly Func<ApiDescription, bool> _showAction;

        private SwaggerAuthorizationFilter(Func<ApiDescription, bool> apiFilter) => _showAction = apiFilter;

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var usedDefinitions = new List<string>();

            IEnumerable<string> GettAllDefinitionsFrom(Operation item)
            {
                var response = new List<string>();

                if (item == null)
                    return response;

                response.AddRange(item.parameters.Where(x => !string.IsNullOrEmpty(x.schema?.@ref)).Select(x => x.schema.@ref.Split('/').LastOrDefault()));

                response.AddRange(item.responses.Where(x => !string.IsNullOrEmpty(x.Value.schema?.@ref)).Select(x => x.Value.schema?.@ref.Split('/').LastOrDefault()));

                return response;
            }

            var descriptions = apiExplorer.ApiDescriptions;
            foreach (var description in descriptions)
            {
                var route = "/" + description.Route.RouteTemplate.TrimEnd('/');
                var path = swaggerDoc.paths[route];

                var hideAction = _showAction?.Invoke(description) ?? false;

                if (!hideAction)
                {
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.delete));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.get));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.head));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.options));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.patch));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.post));
                    usedDefinitions.AddRange(GettAllDefinitionsFrom(path.put));

                    continue;
                }

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

            var removedDefinitions = swaggerDoc.definitions.Keys.Except(usedDefinitions.Distinct()).ToList();

            foreach (var removedDefinition in removedDefinitions)
                swaggerDoc.definitions.Remove(removedDefinition);
        }
    }
}
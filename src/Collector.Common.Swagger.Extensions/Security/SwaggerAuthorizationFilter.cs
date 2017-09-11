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

            var descriptions = apiExplorer.ApiDescriptions;
            foreach (var description in descriptions)
            {
                var route = "/" + description.Route.RouteTemplate.TrimEnd('/');
                var path = swaggerDoc.paths[route];

                var hideAction = _showAction?.Invoke(description) ?? false;

                if (!hideAction)
                {
                    switch (description.HttpMethod.Method)
                    {
                        case "DELETE": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.delete)); break;
                        case "GET": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.get)); break;
                        case "HEAD": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.head)); break;
                        case "OPTIONS": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.options)); break;
                        case "PATCH": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.patch)); break;
                        case "POST": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.post)); break;
                        case "PUT": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.put)); break;
                    }

                    continue;
                }

                HideAction(swaggerDoc, description, path, route);
            }

            var removedDefinitions = swaggerDoc.definitions.Keys.Except(usedDefinitions.Distinct()).ToList();

            foreach (var removedDefinition in removedDefinitions)
                swaggerDoc.definitions.Remove(removedDefinition);
        }

        private static void HideAction(SwaggerDocument swaggerDoc, ApiDescription description, PathItem path, string route)
        {
            switch (description.HttpMethod.Method)
            {
                case "DELETE":
                    path.delete = null;
                    break;
                case "GET":
                    path.get = null;
                    break;
                case "HEAD":
                    path.head = null;
                    break;
                case "OPTIONS":
                    path.options = null;
                    break;
                case "PATCH":
                    path.patch = null;
                    break;
                case "POST":
                    path.post = null;
                    break;
                case "PUT":
                    path.put = null;
                    break;
                default: throw new ArgumentOutOfRangeException("Method name not mapped to operation");
            }

            if (path.delete == null && path.get == null &&
                path.head == null && path.options == null &&
                path.patch == null && path.post == null && path.put == null)
                swaggerDoc.paths.Remove(route);
        }

        private IEnumerable<string> GetAllDefinitionsFrom(Operation item)
        {
            var response = new List<string>();

            if (item == null)
                return response;

            response.AddRange(item.parameters?
                .Where(x => !string.IsNullOrEmpty(x.schema?.@ref))
                .Select(x => x.schema.@ref.Split('/').LastOrDefault()) ?? Enumerable.Empty<string>());

            response.AddRange(item.responses?
                .Where(x => !string.IsNullOrEmpty(x.Value.schema?.@ref))
                .Select(x => x.Value.schema?.@ref.Split('/').LastOrDefault()) ?? Enumerable.Empty<string>());

            return response;
        }
    }
}
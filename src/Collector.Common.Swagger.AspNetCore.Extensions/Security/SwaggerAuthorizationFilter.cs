/*
 https://github.com/jenyayel/SwaggerSecurityTrimming
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Collector.Common.Swagger.AspNetCore.Extensions.Security
{
    public class SwaggerAuthorizationFilter : IDocumentFilter
    {
        private readonly IServiceProvider _provider;

        public SwaggerAuthorizationFilter(IServiceProvider provider)
        {
            this._provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var http = this._provider.GetRequiredService<IHttpContextAccessor>();
            var auth = this._provider.GetRequiredService<IAuthorizationService>();
            
            var usedDefinitions = new List<string>();

            var descriptions = context.ApiDescriptionsGroups.Items.SelectMany(group => group.Items);

            foreach (var description in descriptions)
            {
                var authAttributes = description.ControllerAttributes()
                    .OfType<AuthorizeAttribute>()
                    .Union(description.ActionAttributes()
                        .OfType<AuthorizeAttribute>());

                // check if this action should be visible
                var showCurrentAction = IsUserAllowed(http, authAttributes) &&
                                IsPolicyAllowed(http, auth, authAttributes);
                
                var route = "/" + description.RelativePath.TrimEnd('/');
                var path = swaggerDoc.Paths[route];

                if (showCurrentAction)
                {
                    switch (description.HttpMethod)
                    {
                        case "DELETE": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Delete)); break;
                        case "GET": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Get)); break;
                        case "HEAD": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Head)); break;
                        case "OPTIONS": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Options)); break;
                        case "PATCH": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Patch)); break;
                        case "POST": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Post)); break;
                        case "PUT": usedDefinitions.AddRange(GetAllDefinitionsFrom(path.Put)); break;
                    }

                    continue;
                }

                HideAction(swaggerDoc, description, path, route);
            }
            
            var removedDefinitions = swaggerDoc.Definitions.Keys.Except(usedDefinitions.Distinct()).ToList();

            foreach (var removedDefinition in removedDefinitions)
                swaggerDoc.Definitions.Remove(removedDefinition);
        }

        private static void HideAction(SwaggerDocument swaggerDoc, ApiDescription description, PathItem path, string route)
        {
            // remove method or entire path (if there are no more methods in this path)
            switch (description.HttpMethod)
            {
                case "DELETE":
                    path.Delete = null;
                    break;
                case "GET":
                    path.Get = null;
                    break;
                case "HEAD":
                    path.Head = null;
                    break;
                case "OPTIONS":
                    path.Options = null;
                    break;
                case "PATCH":
                    path.Patch = null;
                    break;
                case "POST":
                    path.Post = null;
                    break;
                case "PUT":
                    path.Put = null;
                    break;
                default: throw new ArgumentOutOfRangeException("Method name not mapped to operation");
            }

            if (path.Delete == null && path.Get == null &&
                path.Head == null && path.Options == null &&
                path.Patch == null && path.Post == null && path.Put == null)
                swaggerDoc.Paths.Remove(route);
        }

        private static IEnumerable<string> GetAllDefinitionsFrom(Operation item)
        {
            var response = new List<string>();

            if (item == null)
                return response;
            
            response.AddRange(item.Schemes?
                                  .Select(x => x.Split('/').LastOrDefault()) ?? Enumerable.Empty<string>());

            response.AddRange(item.Responses?
                                  .Where(x => !string.IsNullOrEmpty(x.Value.Schema?.Ref))
                                  .Select(x => x.Value.Schema?.Ref.Split('/').LastOrDefault()) ?? Enumerable.Empty<string>());

            return response;
        }

        private static bool IsPolicyAllowed(
            IHttpContextAccessor http,
            IAuthorizationService auth,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            var policies = attributes
                .Where(p => !string.IsNullOrEmpty(p.Policy))
                .Select(a => a.Policy)
                .Distinct();
            
            return policies.All(p => Task.Run(async () => await auth.AuthorizeAsync(http.HttpContext.User, p)).Result);
        }

        private static bool IsUserAllowed(
            IHttpContextAccessor http,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            return !attributes.Any() || http.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
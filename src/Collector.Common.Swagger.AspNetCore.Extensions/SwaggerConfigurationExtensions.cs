// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerConfigurationExtensions.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// <summary>
//   The Collector Bank Swagger AspNetCore configuration extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Collector.Common.Swagger.AspNetCore.Extensions.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Collector.Common.Swagger.AspNetCore.Extensions
{
    /// <summary>
    /// The collector swagger configuration extensions.
    /// </summary>
    public static class SwaggerConfigurationExtensions
    {
        /// <summary>
        /// Use this to enable the collector style-sheet
        /// </summary>
        /// <param name="swaggerUiOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void InjectCollectorTheme(this SwaggerUIOptions swaggerUiOptions) =>
            swaggerUiOptions.InjectStylesheet(Constants.Route + Constants.Css);

        /// <summary>
        /// Use this to enable bearer token in user interface.
        /// </summary>
        /// <param name="swaggerUiOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void InjectBearerTokenJs(this SwaggerUIOptions swaggerUiOptions) => 
            swaggerUiOptions.InjectOnCompleteJavaScript(Constants.Route + Constants.Js);

        /// <summary>
        /// Use this to enable bearer token in user interface.
        /// </summary>
        /// <param name="swaggerGenOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableBearerTokenAuthorization(this SwaggerGenOptions swaggerGenOptions) =>
            swaggerGenOptions.AddSecurityDefinition("api_key", new BearerTokenScheme());

        /// <summary>
        /// Use this to enabled the AuthorizationFilter that hides unauthorized endpoints for the consumer.
        /// </summary>
        /// <param name="swaggerGenOptions">
        /// The swagger User Interface Config
        ///</param>
        public static void EnabledAuthorizationFilter(this SwaggerGenOptions swaggerGenOptions) => 
            swaggerGenOptions.DocumentFilter<SwaggerAuthorizationFilter>();
        

        /// <summary>
        /// Adds the Swagger Gen and also enables Bearer Token security definition with authorization filter
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGenWithBearerToken(this IServiceCollection services, Action<SwaggerGenOptions> setupAction)
        {
            if (services.All(x => x.ImplementationType != typeof(IHttpContextAccessor)))
            {
                services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            }
            services.AddSwaggerGen(options =>
            {
                setupAction.Invoke(options);
                options.EnableBearerTokenAuthorization();
            });
            return services;
        }

        /// <summary>
        /// Inject Collector Theme CSS and Bearer Token api_key functionality
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerUIWithCollectorTheme(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> setupAction)
        {
            app.UseSwaggerUI(options =>
            {
                setupAction.Invoke(options);
                options.InjectCollectorTheme();
                options.InjectBearerTokenJs();
            });

            return app;
        }
    }
}
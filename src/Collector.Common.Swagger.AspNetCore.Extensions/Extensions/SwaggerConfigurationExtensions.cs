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
        private const string ActionPath = Constants.Route;
        private const string Css = "Resources.collectortheme.css";
        private const string Js = "Resources.clientcredentials.js";
        /// <summary>
        /// Use this to enable the collector style-sheet
        /// </summary>
        /// <param name="swaggerUiOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableCollectorTheme(this SwaggerUIOptions swaggerUiOptions)
        {
            swaggerUiOptions.InjectStylesheet(ActionPath + Css);
        }

        /// <summary>
        /// Use this to enable bearer token in user interface.
        /// </summary>
        /// <param name="swaggerUiOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableBearerToken(this SwaggerUIOptions swaggerUiOptions)
        {
            swaggerUiOptions.InjectOnCompleteJavaScript(ActionPath + Js);
        }

        /// <summary>
        /// Use this to enable bearer token in user interface.
        /// </summary>
        /// <param name="swaggerGenOptions">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableBearerTokenAuthorization(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.AddSecurityDefinition("api_key", new BaererTokenScheme());
            swaggerGenOptions.DocumentFilter<SwaggerAuthorizationFilter>();
        }

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

        public static IApplicationBuilder UseCollectorSwaggerUI(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> setupAction)
        {

            app.UseSwaggerUI(options =>
            {
                setupAction.Invoke(options);
                options.EnableCollectorTheme();
            });

            return app;
        }

    }
}

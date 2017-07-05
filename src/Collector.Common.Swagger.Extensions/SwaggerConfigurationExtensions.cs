// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerConfigurationExtensions.cs" company="Collector AB">
//   Copyright © Collector AB. All rights reserved.
// </copyright>
// <summary>
//   The collector swagger configuration extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web.Http;

namespace Collector.Common.Swagger.Extensions
{
    using Swashbuckle.Application;
    
    /// <summary>
    /// The collector swagger configuration extensions.
    /// </summary>
    public static class SwaggerConfigurationExtensions
    {
        public static SwaggerEnabledConfiguration EnableSwaggerUiWithCollectorTheme(this SwaggerEnabledConfiguration configuration, Action<SwaggerUiConfig> configure = null)
        {
            configuration.EnableSwaggerUi(config =>
            {
                configure?.Invoke(config);
                config.EnableCollectorTheme();
            });
            return configuration;
        }

        public static SwaggerEnabledConfiguration EnableSwaggerUiWithCollectorTheme(this SwaggerEnabledConfiguration configuration, string routeTemplate, Action<SwaggerUiConfig> configure = null)
        {
            configuration.EnableSwaggerUi(routeTemplate, config =>
            {
                configure?.Invoke(config);
                config.EnableCollectorTheme();
            });
            return configuration;
        }


        /// <summary>
        /// Use this to enable the collector style-sheet
        /// </summary>
        /// <param name="swaggerUiConfig">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableCollectorTheme(this SwaggerUiConfig swaggerUiConfig)
        {
            swaggerUiConfig.InjectStylesheet(typeof(SwaggerConfigurationExtensions).Assembly, "Collector.Common.Swagger.Extensions.Resources.collectortheme.css");
        }

        /// <summary>
        /// Use this to enable bearer token in user interface.
        /// </summary>
        /// <param name="swaggerUiConfig">
        /// The swagger User Interface Config
        /// </param>
        public static void EnableBearerToken(this SwaggerUiConfig swaggerUiConfig)
        {
            swaggerUiConfig.InjectJavaScript(typeof(SwaggerConfigurationExtensions).Assembly, "Collector.Common.Swagger.Extensions.Resources.clientcredentials.js");
        }
    }

}

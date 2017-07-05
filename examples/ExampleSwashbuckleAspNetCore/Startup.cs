using System;
using Collector.Common.Swagger.AspNetCore.Extensions;
using Collector.Common.Swagger.AspNetCore.Extensions.Security;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace ExampleSwashbuckleAspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver());
            services.AddAuthorization(options =>
            {
                options.AddPolicy("example", policy => policy.AddRequirements(new DenyAnonymousAuthorizationRequirement()));
            });


            services.AddSwaggerGenWithBearerToken(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Swagger Extensions Example",
                    Description = "#Markdown enabled description field!" + Environment.NewLine + "* Write your description here",
                    TermsOfService = "None"
                });
                options.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUIWithCollectorTheme(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Example Api v1");
            });
        }
    }
}

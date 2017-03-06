using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace log4net.Raygun.DotNetCore
{
    public class Log4NetRaygunMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RaygunMiddlewareSettings _middlewareSettings;
        private readonly RaygunSettings _settings;

        public Log4NetRaygunMiddleware(RequestDelegate next, IOptions<RaygunSettings> settings, RaygunMiddlewareSettings middlewareSettings)
        {
            _next = next;
            _middlewareSettings = middlewareSettings;

            _settings = _middlewareSettings.ClientProvider.GetRaygunSettings(settings.Value ?? new RaygunSettings());
        }
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                var client = _middlewareSettings.ClientProvider.GetClient(_settings, httpContext);
                await client.SendInBackground(e);
                throw;
            }
        }
    }
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRaygun(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RaygunAspNetMiddleware>();
        }

        public static IServiceCollection AddLog4NetRaygun(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<RaygunSettings>(configuration.GetSection("RaygunSettings"));

            services.AddTransient<IRaygunAspNetCoreClientProvider>(_ => new DefaultRaygunAspNetCoreClientProvider());
            services.AddSingleton<RaygunMiddlewareSettings>();

            return services;
        }

        public static IServiceCollection AddLog4NetRaygun(this IServiceCollection services, IConfiguration configuration, RaygunMiddlewareSettings middlewareSettings)
        {
            services.Configure<RaygunSettings>(configuration.GetSection("RaygunSettings"));

            services.AddTransient(_ => middlewareSettings.ClientProvider ?? new DefaultRaygunAspNetCoreClientProvider());
            services.AddTransient(_ => middlewareSettings);

            return services;
        }
    }
}

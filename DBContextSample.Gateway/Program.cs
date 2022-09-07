using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace DBContextSample.Gateway
{
    public partial class Program
    {
        public static async Task Main(string[] args)
            => await GetApp(args).RunAsync();

        public static WebApplication GetApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Before build

            builder.Configuration
                .AddJsonFile("gateway.settings.json", false, false)
                .AddJsonFile("gateway.hosting.json", false, false)
                .AddJsonFile("gateway.ocelot.json", false, false);

            builder.Services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services
                .AddOcelot();

            #endregion

            var app = builder.Build();

            #region After build

            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseOcelot().Wait();

            #endregion

            loggerFactory
                .CreateLogger("Program")
                .Warning("Gateway: I started !");

            return app;
        }
    }
}
using DBContextSample.API.Services;
using DBContextSample.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DBContextSample.Test
{
    internal class ApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json", true, true)
                .Build();

            builder
                .ConfigureServices(services =>
                {
                    services
                        .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                    services
                        .AddDbContext<CoreContext>(
                            (serviceProvider, options) =>
                                options.UseSqlServer(
                                    configuration["ConnectionStrings:Default"]
                                ),
                            contextLifetime: ServiceLifetime.Scoped,
                            optionsLifetime: ServiceLifetime.Singleton
                        );

                    services
                        .RegisterServices();
                });

            return base.CreateHost(builder);
        }
    }
}

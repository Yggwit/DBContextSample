using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DBContextSample.Gateway.Test
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
                });

            return base.CreateHost(builder);
        }
    }
}

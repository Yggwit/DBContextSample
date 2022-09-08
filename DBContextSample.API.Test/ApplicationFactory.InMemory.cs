using DBContextSample.Context;
using DBContextSample.Context.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DBContextSample.API.Test
{
    internal class InMemoryApplicationFactory : ApplicationFactory
    {
        private readonly string? _databaseName = null;

        internal InMemoryApplicationFactory(string? databaseName = null)
            => _databaseName = databaseName;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CoreContext>))
                    is ServiceDescriptor descriptor
                )
                    services.Remove(descriptor);

                services.AddInMemoryDbContext<CoreContext>(_databaseName);
            });
        }
    }
}

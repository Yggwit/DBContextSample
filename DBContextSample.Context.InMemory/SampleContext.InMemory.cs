using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DBContextSample.Context.InMemory
{
    public static class InMemoryContext
    {
        public static IServiceCollection AddInMemoryDbContext<TContext>
            (this IServiceCollection services, string? databaseName = null)
            where TContext : DbContext
            => services.AddDbContext<TContext>(options =>
                options.UseInMemoryDatabase(databaseName: databaseName ?? Guid.NewGuid().ToString())
            );
    }
}
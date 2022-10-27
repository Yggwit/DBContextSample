namespace DBContextSample.API.Services
{
    public static partial class IServiceCollectionExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
            => services
                .AddScoped<IFakeService, FakeService>()
                .AddScoped<FilterService<CoreContext>>();
    }
}

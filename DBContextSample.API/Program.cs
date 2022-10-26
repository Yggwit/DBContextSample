using DBContextSample.API.Services;
using DBContextSample.API.Sieve;
using Sieve.Models;
using Sieve.Services;

namespace DBContextSample.API
{
    public partial class Program
    {
        public static async Task Main(string[] args)
            => await GetApp(args).RunAsync();

        public static WebApplication GetApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Before build

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration
                .AddJsonFile("api.settings.json", false, true)
                .AddJsonFile("api.hosting.json", false, true);

            builder.Services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services
                .AddHealthChecks()
                .AddDbContextCheck<CoreContext>();

            builder.Services
                .AddDbContext<CoreContext>(
                    (serviceProvider, options) =>
                        options.UseSqlServer(
                            builder.Configuration["ConnectionStrings:Default"]
                        ),
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Singleton
                );

            builder.Services
                .RegisterServices();


            builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
            builder.Services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

            #endregion

            var app = builder.Build();

            #region After build

            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));
            DbContextLogger.LoggerFactory = loggerFactory;

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/api/health");

            #endregion

            loggerFactory
                .CreateLogger("Program")
                .Warning("I started !");

            return app;
        }
    }
}

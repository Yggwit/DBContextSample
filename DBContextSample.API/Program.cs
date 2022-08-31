var builder = WebApplication.CreateBuilder(args);

#region Before build

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .AddJsonFile("hosting.json", optional: false);

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services
    .AddDbContext<CoreContext>(
        (serviceProvider, options) =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:Default"]
            ),
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Singleton
    );

#endregion

var app = builder.Build();

#region After build

var configuration = app.Services.GetRequiredService<IConfiguration>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));
DbContextLogger.LoggerFactory = loggerFactory;

#endregion


app.Run();
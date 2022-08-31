global using DBContextSample.Context.Helpers;
global using DBContextSample.Context;
global using Microsoft.EntityFrameworkCore;
using DBContextSample.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("hosting.json", optional: false);

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services
    .AddDbContext<CoreContext>(
        (serviceProvider, options) =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:Default"]
            )
    );


var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();



var configuration = app.Services.GetRequiredService<IConfiguration>();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));
DbContextLogger.LoggerFactory = loggerFactory;


app.Run();

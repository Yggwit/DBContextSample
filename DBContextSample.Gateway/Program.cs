using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);

#region Before build

builder.Configuration
    .AddJsonFile("hosting.json", false, true)
    .AddJsonFile("ocelot.json");

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services
    .AddOcelot();

#endregion

var app = builder.Build();

#region After build

var configuration = app.Services.GetRequiredService<IConfiguration>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseMiddleware<RequestLoggingMiddleware>();

loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));

app.UseOcelot().Wait();

#endregion


app.Run();






//namespace DBContextSample.Gateway
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            new WebHostBuilder()
//                .UseKestrel()
//                .UseContentRoot(Directory.GetCurrentDirectory())
//                .ConfigureAppConfiguration((hostingContext, config) =>
//                {
//                    config
//                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
//                        .AddJsonFile("hosting.json", false, true)
//                        .AddJsonFile("appsettings.json", true, true)
//                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
//                        .AddJsonFile("ocelot.json")
//                        .AddEnvironmentVariables();
//                })
//                .ConfigureServices(s =>
//                {
//                    s.AddOcelot();
//                })
//                .ConfigureLogging((hostingContext, logging) =>
//                {
//                    //add your logging
//                })
//                .UseIISIntegration()
//                .Configure(app =>
//                {
//                    app.UseOcelot().Wait();
//                })
//                .Build()
//                .Run();
//        }
//    }
//}
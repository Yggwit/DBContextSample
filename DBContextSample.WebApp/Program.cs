var builder = WebApplication.CreateBuilder(args);

#region Before build

builder.Services.AddRazorPages();

builder.Configuration
    .AddJsonFile("hosting.json", false, true);

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#endregion

var app = builder.Build();

#region After build

var configuration = app.Services.GetRequiredService<IConfiguration>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseMiddleware<RequestLoggingMiddleware>();

loggerFactory.AddSeq(configuration.GetSection("Logging:Seq"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

#endregion

loggerFactory
    .CreateLogger("Program")
    .Warning("I started !");

app.Run();

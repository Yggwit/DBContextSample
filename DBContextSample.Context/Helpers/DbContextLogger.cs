using Microsoft.Extensions.Logging;

namespace DBContextSample.Context.Helpers
{
    public static class DbContextLogger
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILogger CreateLogger<T>() => LoggerFactory?.CreateLogger<T>();
    }
}

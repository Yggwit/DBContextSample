using Microsoft.Extensions.Logging;

namespace DBContextSample.Utils
{
    public static class ILoggerExtensions
    {
#pragma warning disable CA2254 // Template should be a static expression
        public static void Trace(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Trace, message, args);

        public static void Debug(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Debug, message, args);

        public static void Information(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Information, message, args);

        public static void Warning(this ILogger logger, Exception ex, params object?[] args)
            => logger.Log(LogLevel.Warning, ex, ex.Message, args);
        public static void Warning(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Warning, message, args);

        public static void Error(this ILogger logger, Exception ex, params object?[] args)
            => logger.Log(LogLevel.Error, ex, ex.Message, args);

        public static void Error(this ILogger logger, Exception ex, string message, params object?[] args)
           => logger.Log(LogLevel.Error, ex, message, args);
        public static void Error(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Error, message, args);

        public static void Critical(this ILogger logger, Exception ex, params object?[] args)
            => logger.Log(LogLevel.Critical, ex, ex.Message, args);
        public static void Critical(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.Critical, message, args);

        public static void None(this ILogger logger, string message, params object?[] args)
            => logger.Log(LogLevel.None, message, args);
#pragma warning restore CA2254 // Template should be a static expression
    }
}
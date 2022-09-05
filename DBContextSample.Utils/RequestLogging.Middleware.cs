using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace DBContextSample.Utils
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IHttpContextAccessor _httpContext;


        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IHttpContextAccessor httpContext)
        {
            _next = next;
            _logger = logger;
            _httpContext = httpContext;
        }

        public async Task Invoke(HttpContext context)
        {
            DateTime startTime = DateTime.UtcNow;

            MemoryStream? injectedRequestStream = null;
            string? parameters = (context.Request.ContentLength ?? 0) > 0
                ? GetRequestBody(context, out injectedRequestStream)
                : context.Request.QueryString.Value;

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                watch.Stop();

                LogRequest(new RequestLogging
                {
                    StartDate = startTime,
                    Elapsed = watch.Elapsed,
                    UserLogon = _httpContext?.HttpContext?.User?.Identity?.Name ?? default!,
                    Method = context.Request.Method,
                    RequestPath = context.Request.Path,
                    RequestHost = context.Request.Host.ToString(),
                    Parameters = parameters,
                    StatusCode = (short)context.Response.StatusCode
                });

                #region Dispose stream
                if (injectedRequestStream != null)
                    injectedRequestStream.Dispose();
                #endregion
            }
        }

        private string GetRequestBody(HttpContext context, out MemoryStream injectedRequestStream)
        {
            string body = string.Empty;
            injectedRequestStream = new MemoryStream();

            try
            {
                using StreamReader bodyReader = new(context.Request.Body);
                body = bodyReader.ReadToEnd();

                byte[] bytesToWrite = Encoding.UTF8.GetBytes(body);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = injectedRequestStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Error on {Method}; {@Exception}",
                    $"{ToString()}.{nameof(GetRequestBody)}",
                    ex
                );
            }

            return body;
        }

        private void LogRequest(RequestLogging request)
        {
            (string message, object?[] args) = request.GetLog();
            _logger.Information(message, args);
        }
    }

    sealed class RequestLogging
    {
        public DateTime StartDate { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string? UserLogon { get; set; }
        public string? Method { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestHost { get; set; }
        public string? Parameters { get; set; }
        public short StatusCode { get; set; }

        public (string, object?[]) GetLog()
        {
            var log = typeof(RequestLogging)
                .GetProperties()
                .Select(s => new
                {
                    Message = $"{s.Name}: {{{s.Name}}};",
                    Arg = s.GetValue(this)
                });

            return (
                string.Join(" ", log.Select(l => l.Message)),
                log.Select(l => l.Arg).ToArray()
            );
        }
    }
}

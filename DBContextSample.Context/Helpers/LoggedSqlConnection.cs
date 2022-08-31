using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace DBContextSample.Context.Helpers
{
    internal class LoggedSqlConnection : IDisposable
    {
        private readonly ILogger _logger;
        private readonly DbContext _context;
        private readonly Stopwatch _stopwatch = new();
        private readonly LogStat _logStat;

        public SqlConnection SqlConnection => _context.SqlConnection();


        public LoggedSqlConnection(DbContext context, ILogger logger, string sql, object parameters = null)
        {
            _context = context;

            if (logger is null)
                return;


            _logger = logger;
            _logStat = new()
            {
                Sql = sql,
                Parameters = GetParameters(parameters)
            };

            _stopwatch.Start();
        }


        public void Dispose()
        {
            if (_logger is null)
                return;

            _stopwatch.Stop();

            _logStat.Elapsed = _stopwatch.ElapsedMilliseconds;

            (string message, object[] args) = _logStat.GetLog();

            _logger.Information(
                $"Executed Query {message}",
                args
            );
        }


        private static string GetParameters(object parameters)
        {
            if (parameters is null)
                return null;

            else if (parameters is DynamicParameters dynamicParameters)
            {
                Dictionary<string, object> parameters_ = new();

                foreach (var paramName in dynamicParameters.ParameterNames)
                {
                    dynamic param = dynamicParameters.Get<dynamic>(paramName);
                    parameters_.Add(
                        paramName,
                        param is SqlMapper.ICustomQueryParameter
                            ? "TableValuedParameter"
                            : param
                    );
                }

                return JsonSerializer.Serialize(parameters_);
            }

            else
                return JsonSerializer.Serialize(parameters);
        }

        private class LogStat
        {
            public string Sql { private get; set; }
            public long? Elapsed { private get; set; }
            public string Parameters { private get; set; }

            internal static string GetParameters(object parameters)
            {
                if (parameters is null)
                    return null;

                else if (parameters is DynamicParameters dynamicParameters)
                {
                    Dictionary<string, object> parameters_ = new();

                    foreach (var paramName in dynamicParameters.ParameterNames)
                        parameters_.Add(paramName, dynamicParameters.Get<dynamic>(paramName));

                    return JsonSerializer.Serialize(parameters_);
                }

                else
                    return JsonSerializer.Serialize(parameters);
            }

            internal (string, object[]) GetLog()
            {
                var log = typeof(LogStat)
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
}

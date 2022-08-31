using Dapper;
using DBContextSample.Context.Helpers;
using DBContextSample.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DBContextSample.Context
{
    public static class DbContextExecuteExtention
    {
        private static readonly ILogger _logger = DbContextLogger.CreateLogger<SampleContext>();

        private static DynamicParameters NormalizeDynamicParameters(DynamicParameters parameters)
        {
            foreach (var paramName in parameters.ParameterNames)
            {
                var paramValue = parameters.Get<dynamic>(paramName);

                if (
                    paramValue?.GetType() == typeof(string)
                    && paramValue != null
                    && paramValue == ""
                )
                {
                    paramValue = null;
                    parameters.Add(name: paramName, value: paramValue, dbType: DbType.String, size: 4000);
                }
            }

            return parameters;
        }


        #region ExecuteQuery

        public static void ExecuteQuery(this DbContext context, string query, int? commandTimeout = null)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, query);

            loggedSqlConnection.SqlConnection
                .Query(
                    query,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        #endregion


        #region ExecuteQueryAsync<T>

        public static async Task ExecuteQueryAsync(this DbContext context, string query, int? commandTimeout = null)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, query);

            await loggedSqlConnection.SqlConnection
                .QueryAsync(
                    query,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        #endregion


        #region ExecuteQuery<T>

        public static IEnumerable<T> ExecuteQuery<T>(this DbContext context, string query, int? commandTimeout = null)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, query);

            return loggedSqlConnection.SqlConnection
                .Query<T>(
                    query,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        #endregion


        #region ExecuteQueryAsync<T>

        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(this DbContext context, string query, int? commandTimeout = null)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, query);

            return await loggedSqlConnection.SqlConnection
                .QueryAsync<T>(
                    query,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        #endregion


        #region Execute

        private static void Execute_Impl(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            loggedSqlConnection.SqlConnection
                .Query(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static void Execute(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            => Execute_Impl(context, schema, sproc, commandTimeout, parameters);
        public static void Execute(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters, bool normalize = true)
            => Execute_Impl(context, schema, sproc, commandTimeout, normalize ? NormalizeDynamicParameters(parameters) : parameters);
        public static void Execute(this DbContext context, string schema, string sproc, object parameters = null)
            => Execute_Impl(context, schema, sproc, null, parameters);
        public static void Execute(this DbContext context, string schema, string sproc, DynamicParameters parameters, bool normalize = true)
            => Execute_Impl(context, schema, sproc, null, normalize ? NormalizeDynamicParameters(parameters) : parameters);

        #endregion


        #region ExecuteAsync

        private static async Task ExecuteAsync_Impl(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            await loggedSqlConnection.SqlConnection
                .QueryAsync(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static async Task ExecuteAsync(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            => await ExecuteAsync_Impl(context, schema, sproc, commandTimeout, parameters);
        public static async Task ExecuteAsync(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters)
            => await ExecuteAsync_Impl(context, schema, sproc, commandTimeout, NormalizeDynamicParameters(parameters));
        public static async Task ExecuteAsync(this DbContext context, string schema, string sproc, object parameters = null)
            => await ExecuteAsync_Impl(context, schema, sproc, null, parameters);
        public static async Task ExecuteAsync(this DbContext context, string schema, string sproc, DynamicParameters parameters)
            => await ExecuteAsync_Impl(context, schema, sproc, null, NormalizeDynamicParameters(parameters));

        #endregion


        #region Execute<T>

        private static IEnumerable<T> Execute_Impl<T>(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters)
            where T : class
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            return loggedSqlConnection.SqlConnection
                .Query<T>(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static IEnumerable<T> Execute<T>(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            where T : class
            => Execute_Impl<T>(context, schema, sproc, commandTimeout, parameters);
        public static IEnumerable<T> Execute<T>(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters, bool normalize = true)
            where T : class
            => Execute_Impl<T>(context, schema, sproc, commandTimeout, normalize ? NormalizeDynamicParameters(parameters) : parameters);
        public static IEnumerable<T> Execute<T>(this DbContext context, string schema, string sproc, object parameters = null)
            where T : class
            => Execute_Impl<T>(context, schema, sproc, null, parameters);
        public static IEnumerable<T> Execute<T>(this DbContext context, string schema, string sproc, DynamicParameters parameters, bool normalize = true)
            where T : class
            => Execute_Impl<T>(context, schema, sproc, null, normalize ? NormalizeDynamicParameters(parameters) : parameters);

        #endregion


        #region ExecuteAsync<T>

        private static async Task<IEnumerable<T>> ExecuteAsync_Impl<T>(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters)
            where T : class
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            return await loggedSqlConnection.SqlConnection
                .QueryAsync<T>(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static async Task<IEnumerable<T>> ExecuteAsync<T>(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            where T : class
            => await ExecuteAsync_Impl<T>(context, schema, sproc, commandTimeout, parameters);
        public static async Task<IEnumerable<T>> ExecuteAsync<T>(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters, bool normalize = true)
            where T : class
            => await ExecuteAsync_Impl<T>(context, schema, sproc, commandTimeout, normalize ? NormalizeDynamicParameters(parameters) : parameters);
        public static async Task<IEnumerable<T>> ExecuteAsync<T>(this DbContext context, string schema, string sproc, object parameters = null)
            where T : class
            => await ExecuteAsync_Impl<T>(context, schema, sproc, null, parameters);
        public static async Task<IEnumerable<T>> ExecuteAsync<T>(this DbContext context, string schema, string sproc, DynamicParameters parameters, bool normalize = true)
            where T : class
            => await ExecuteAsync_Impl<T>(context, schema, sproc, null, normalize ? NormalizeDynamicParameters(parameters) : parameters);

        #endregion


        #region ExecuteMultiple

        private static SqlMapper.GridReader ExecuteMultiple_Impl(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters)
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            return loggedSqlConnection.SqlConnection
                .QueryMultiple(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static SqlMapper.GridReader ExecuteMultiple(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            => ExecuteMultiple_Impl(context, schema, sproc, commandTimeout, parameters);
        public static SqlMapper.GridReader ExecuteMultiple(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters)
            => ExecuteMultiple_Impl(context, schema, sproc, commandTimeout, parameters);
        public static SqlMapper.GridReader ExecuteMultiple(this DbContext context, string schema, string sproc, object parameters = null)
            => ExecuteMultiple_Impl(context, schema, sproc, null, parameters);
        public static SqlMapper.GridReader ExecuteMultiple(this DbContext context, string schema, string sproc, DynamicParameters parameters)
            => ExecuteMultiple_Impl(context, schema, sproc, null, NormalizeDynamicParameters(parameters));

        #endregion


        #region ExecuteMultipleAsync

        private static async Task<SqlMapper.GridReader> ExecuteMultipleAsync_Impl<T>(this T context, string schema, string sproc, int? commandTimeout, object parameters)
            where T : DbContext
        {
            using LoggedSqlConnection loggedSqlConnection = new(context, _logger, $"{schema}.{sproc}", parameters);

            return await loggedSqlConnection.SqlConnection
                .QueryMultipleAsync(
                    sql: $"{schema}.{sproc}",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout,
                    transaction: context is ITransactionContext transactionContext
                        ? transactionContext.DbTransaction()
                        : null
                );
        }

        public static async Task<SqlMapper.GridReader> ExecuteMultipleAsync(this DbContext context, string schema, string sproc, int? commandTimeout, object parameters = null)
            => await ExecuteMultipleAsync_Impl(context, schema, sproc, commandTimeout, parameters);
        public static async Task<SqlMapper.GridReader> ExecuteMultipleAsync(this DbContext context, string schema, string sproc, int? commandTimeout, DynamicParameters parameters)
            => await ExecuteMultipleAsync_Impl(context, schema, sproc, commandTimeout, NormalizeDynamicParameters(parameters));
        public static async Task<SqlMapper.GridReader> ExecuteMultipleAsync(this DbContext context, string schema, string sproc, object parameters = null)
            => await ExecuteMultipleAsync_Impl(context, schema, sproc, null, parameters);
        public static async Task<SqlMapper.GridReader> ExecuteMultipleAsync(this DbContext context, string schema, string sproc, DynamicParameters parameters)
            => await ExecuteMultipleAsync_Impl(context, schema, sproc, null, NormalizeDynamicParameters(parameters));

        #endregion
    }
}

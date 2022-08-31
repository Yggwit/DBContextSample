using DBContextSample.Entities.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DBContextSample.Context
{
    public static class SampleContextUpdateExtention
    {
        private static StringBuilder GetSyncColumns(SampleContext context, Type type, string tableAlias)
        {
            PropertyInfo TimeModif = type.GetProperty("TimeModif", BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo UserID = type.GetProperty("UserID", BindingFlags.Public | BindingFlags.Instance);

            string timeModif = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int currentUserID = context.CurrentUserID ?? 0;

            StringBuilder columns = new();

            if (TimeModif?.CanWrite ?? false)
                columns.Append($"[{tableAlias}].TimeModif = '{timeModif}'");

            if (UserID?.CanWrite ?? false)
                columns.Append($"{(columns.Length > 0 ? " , " : string.Empty)}[{tableAlias}].UserID = '{currentUserID}'");


            return (
                columns
            );
        }

        public static int Update<T>(this IQueryable<T> query, Expression<Func<T, T>> updateExpression)
            where T : EntityBase, new()
        {
            SampleContext context = (SampleContext)BatchUtil.GetDbContext(query);
            System.Type type = typeof(T);


            /*
                Source: EFCore.BulkExtensions.BatchUpdate
                Ajout des champs de synchro *syncColumns*
            */

            (string sql, string tableAlias, string tableAliasSufixAs, string topStatement, string leadingComments, IEnumerable<object> innerParameters) = BatchUtil.GetBatchSql(query, context, isUpdate: true);

            BatchUpdateCreateBodyData createUpdateBodyData = new(sql, context, innerParameters, query, type, tableAlias, updateExpression);

            BatchUtil.CreateUpdateBody(createUpdateBodyData, updateExpression.Body);

            List<object> sqlParameters = BatchUtil.ReloadSqlParameters(context, createUpdateBodyData.SqlParameters);
            StringBuilder sqlColumns = (createUpdateBodyData.DatabaseType == EFCore.BulkExtensions.SqlAdapters.DbServer.SQLServer)
                ? createUpdateBodyData.UpdateColumnsSql
                : createUpdateBodyData.UpdateColumnsSql.Replace($"[{tableAlias}].", "");

            StringBuilder syncColumns = GetSyncColumns(context, type, tableAlias);

            string columnsSeparator = sqlColumns.Length > 0 && syncColumns.Length > 0
                ? " , "
                : string.Empty;

            string resultQuery = $"{leadingComments}UPDATE {topStatement}{tableAlias}{tableAliasSufixAs} SET {sqlColumns}{columnsSeparator}{syncColumns} {sql}";

            return context.Database.ExecuteSqlRaw(resultQuery, sqlParameters);
        }
    }
}

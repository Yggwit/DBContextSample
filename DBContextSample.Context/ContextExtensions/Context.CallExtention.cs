using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DBContextSample.Context.ContextExtensions
{
    public static class ContextCallExtention
    {
        private static (SqlParameter[] SqlParameters, string QueryParameters) NormalizeSqlParams(params SqlParameter[] sqlParams)
        {
            List<SqlParameter> sqlParameters = sqlParams
                .Where(p => p != null)
                .Select(p =>
                {
                    p.Value = p.Value != null && p.Value is string
                        ? string.IsNullOrWhiteSpace(p.Value.ToString()) ? DBNull.Value : p.Value
                        : p.Value ?? DBNull.Value;

                    return p;
                })
                .ToList();

            List<string> queryParamList = sqlParameters
                .Select(p => p.ParameterName)
                .ToList();


            return (
                SqlParameters: sqlParameters.ToArray(),
                QueryParameters: string.Join(", ", queryParamList)
            );
        }


        #region Call

        public static T Call<T>(this DbContext context, string schema, string fct, params SqlParameter[] sqlParams)
        {
            using SqlCommand command = context.SqlConnection().CreateCommand();

            (SqlParameter[] sqlParameters, string queryParameters) = NormalizeSqlParams(sqlParams);

            command.CommandText = $"SELECT {schema}.{fct}({queryParameters})";
            command.CommandType = CommandType.Text;

            command.Parameters.AddRange(sqlParameters);

            context.Database.OpenConnection();


            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);

            reader.Read();

            var result = reader[0];

            reader.Close();

            return (T)result;
        }

        #endregion
    }
}

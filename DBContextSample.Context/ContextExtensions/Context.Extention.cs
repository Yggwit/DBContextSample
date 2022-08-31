using Microsoft.Data.SqlClient;

namespace DBContextSample.Context
{
    public static class ContextExtention
    {
        public static SqlConnection SqlConnection<T>(this T context)
            where T : DbContext
            => context.Database.GetDbConnection() as SqlConnection;
    }
}

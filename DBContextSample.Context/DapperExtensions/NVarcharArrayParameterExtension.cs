using Dapper;
using System.Data;

namespace DBContextSample.Context
{
    public static class NVarcharArrayParameterExtension
    {
        private static DataTable GetTable()
        {
            DataTable t = new();
            t.Columns.Add(new DataColumn("ID", typeof(string)));

            return t;
        }

        public static void AddNVarcharArrayParameter(this DynamicParameters parameters, string name, IEnumerable<int> values, ParameterDirection? direction = null)
            => AddNVarcharArrayParameter(parameters, name, values.Select(e => e.ToString()), direction);

        public static void AddNVarcharArrayParameter(this DynamicParameters parameters, string name, IEnumerable<string> values, ParameterDirection? direction = null)
        {
            DataTable table = GetTable();
            values
                ?.ToList()
                ?.ForEach(v => table.Rows.Add(v));

            parameters.Add(
                name: name,
                value: table.AsTableValuedParameter("dbo.NVarcharArray"),
                direction: direction
            );
        }
    }
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public interface ISqlCommandWrapper : IDisposable
    {
        SqlCommand Command { get; }

        ISqlCommandWrapper WithParameter<T>(string name, T value, SqlDbType? type = null, int? size = null);
        ISqlCommandWrapper WithBinaryParameter(string name, byte[] data);
        ISqlCommandWrapper WithTableParameter(string name, string typeName, DataTable table);

        Task<int> ExecuteNonQueryAsync();

        Task<SqlDataReader> ExecuteReaderAsync(CommandBehavior behavior = CommandBehavior.Default);
        SqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default);

        Task<object> ExecuteScalarAsync();
    }
}

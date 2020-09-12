using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AntiHarassment.Sql
{
    public class SqlCommandWrapper : ISqlCommandWrapper
    {
        private SqlCommandWrapper(string connectionString)
        {
            Command = new SqlConnection(connectionString).CreateCommand();
        }

        public SqlCommand Command { get; private set; }

        public void Dispose()
        {
            Command.Connection.Dispose();
            Command.Dispose();
        }

        public virtual ISqlCommandWrapper AsStoredProcedure(string sql)
        {
            Command.CommandText = sql;
            Command.CommandType = CommandType.StoredProcedure;
            return this;
        }

        public virtual ISqlCommandWrapper AsQuery(string sql)
        {
            Command.CommandText = sql;
            Command.CommandType = CommandType.Text;
            return this;
        }

        public virtual ISqlCommandWrapper WithParameter<T>(string name, T value, SqlDbType? type = null, int? size = null)
        {
            if (!name.StartsWith("@"))
                name = "@" + name;

            SqlParameter param;
            if (value != null)
                param = Command.Parameters.AddWithValue(name, value);
            else
                param = Command.Parameters.AddWithValue(name, DBNull.Value);

            if (type.HasValue)
                param.SqlDbType = type.Value;

            if (size.HasValue)
                param.Size = size.Value;

            return this;
        }

        public virtual ISqlCommandWrapper WithBinaryParameter(string name, byte[] data)
        {
            var param = new SqlParameter(name, SqlDbType.VarBinary)
            {
                Value = data ?? (object)DBNull.Value
            };

            Command.Parameters.Add(param);

            return this;
        }

        public virtual ISqlCommandWrapper WithTableParameter(string name, string typeName, DataTable table)
        {
            var param = new SqlParameter(name, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = table ?? (object)DBNull.Value
            };

            Command.Parameters.Add(param);

            return this;
        }

        public virtual ISqlCommandWrapper WithTableParameter(string name, string typeName, Func<DataTable> tableFactory)
        {
            return WithTableParameter(name, typeName, tableFactory.Invoke());
        }

        public static SqlCommandWrapper Create(string connectionString)
        {
            return new SqlCommandWrapper(connectionString);
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            await EnsureOpenConnection().ConfigureAwait(false);
            return await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public async Task<object> ExecuteScalarAsync()
        {
            await EnsureOpenConnection().ConfigureAwait(false);
            return await Command.ExecuteScalarAsync().ConfigureAwait(false);
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(CommandBehavior behavior = CommandBehavior.Default)
        {
            await EnsureOpenConnection().ConfigureAwait(false);
            return await Command.ExecuteReaderAsync(behavior).ConfigureAwait(false);
        }

        public SqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        {
            try
            {
                EnsureOpenConnection().Wait();
                return Command.ExecuteReader(behavior);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        private async Task EnsureOpenConnection()
        {
            if (Command.Connection.State != ConnectionState.Open)
                await Command.Connection.OpenAsync().ConfigureAwait(false);
        }
    }
}

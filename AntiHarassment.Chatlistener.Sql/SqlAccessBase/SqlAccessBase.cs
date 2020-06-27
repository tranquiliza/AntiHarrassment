using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Chatlistener.Sql
{
    public abstract class SqlAccessBase : ISqlAccess
    {
        private class SqlAccessImpl : SqlAccessBase
        {
            public SqlAccessImpl(string connectionString) : base(connectionString) { }
        }

        private readonly string _connectionString;

        protected SqlAccessBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ISqlCommandWrapper CreateStoredProcedure(string sql)
        {
            return SqlCommandWrapper.Create(_connectionString)
                .AsStoredProcedure(sql);
        }

        public ISqlCommandWrapper CreateQuery(string sql)
        {
            return SqlCommandWrapper.Create(_connectionString)
                .AsQuery(sql);
        }

        public static ISqlAccess Create(string connectionString)
        {
            return new SqlAccessImpl(connectionString);
        }
    }
}

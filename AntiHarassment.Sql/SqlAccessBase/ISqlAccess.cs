using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Sql
{
    public interface ISqlAccess
    {
        ISqlCommandWrapper CreateStoredProcedure(string sql);
        ISqlCommandWrapper CreateQuery(string sql);
    }
}

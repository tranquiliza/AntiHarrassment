using System;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace AntiHarassment.Sql
{
    public static class SqlDataReaderExtensions
    {
        private static T GetTypedValue<T>(SqlDataReader rdr, string column, Func<object, T> converter)
        {
            var value = rdr[column];
            if (value == DBNull.Value)
                return default;

            return converter(value);
        }

        public static bool GetBoolean(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToBoolean);

        public static byte GetByte(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToByte);

        public static int GetInt32(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToInt32);

        public static long GetInt64(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToInt64);

        public static string GetString(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToString);

        public static DateTime GetDateTime(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToDateTime);

        public static double GetDouble(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, Convert.ToDouble);

        public static byte[] GetByteArray(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, ConvertByteArray);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ConvertByteArray(object value)
            => (byte[])value;

        public static Guid GetGuid(this SqlDataReader rdr, string column)
            => GetTypedValue(rdr, column, ConvertGuid);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Guid ConvertGuid(object value)
        {
            if (value is string txt)
                return Guid.Parse(txt);

            return (Guid)value;
        }
    }
}

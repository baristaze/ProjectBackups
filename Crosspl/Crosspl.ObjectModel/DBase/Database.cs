using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Crosspl.ObjectModel
{
    public class Database
    {
        public const int TimeoutSecs = 30;
        public const int TimeoutSecsForFile = 60;

        public delegate T SqlTypeConverter<T>(SqlDataReader reader, int colIndex);

        #region Select

        public static bool Select(IEditable entity, string connectionString)
        {
            return Select(entity, connectionString, Database.TimeoutSecs);
        }

        public static bool Select(IEditable entity, string connectionString, int timeoutSeconds)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return Select(entity, connection, null, timeoutSeconds);
            }
        }

        public static bool Select(IEditable entity, SqlConnection connection, SqlTransaction transaction, int timeoutSeconds)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = entity.SelectQuery();
                command.Transaction = transaction;
                command.CommandTimeout = timeoutSeconds;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            entity.InitFromSqlReader(reader);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion // Select

        #region SelectAll

        public static L SelectAll<T, L>(string connectionString, object filter) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            return SelectAll<T, L>(connectionString, filter, Database.TimeoutSecs);
        }

        public static L SelectAll<T, L>(string connectionString, object filter, int timeoutSeconds) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return SelectAll<T, L>(connection, null, filter, timeoutSeconds);
            }
        }

        public static L SelectAll<T, L>(SqlConnection connection, SqlTransaction transaction, object filter, int timeoutSeconds) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            string query = (new T()).SelectAllQuery(filter);
            return SelectAll<T, L>(connection, transaction, query, timeoutSeconds);
        }

        public static L SelectAll<T, L>(SqlConnection connection, SqlTransaction transaction, string query, int timeoutSeconds)
            where T : IEditable, new()
            where L : List<T>, new()
        {
            L entities = new L();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = transaction;
                command.CommandTimeout = timeoutSeconds;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            T item = new T();
                            item.InitFromSqlReader(reader);
                            entities.Add(item);
                        }
                    }
                }
            }

            return entities;
        }

        #endregion // Select

        #region InsertOrUpdate

        public static int InsertOrUpdate(IEditable entity, SqlConnection connection, SqlTransaction transaction, int timeoutSeconds)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = entity.InsertOrUpdateQuery();
                command.Transaction = transaction;
                command.CommandTimeout = timeoutSeconds;
                entity.AddSqlParameters(command, DBaseOperation.InsertOrUpdate);

                return command.ExecuteNonQuery();
            }
        }

        public static int InsertOrUpdate(IEditable entity, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return InsertOrUpdate(entity, connection, null, Database.TimeoutSecs);
            }
        }

        public static int InsertOrUpdate(IEnumerable<IEditable> entities, string connectionString)
        {
            return InsertOrUpdate(entities, connectionString, Database.TimeoutSecs);
        }

        public static int InsertOrUpdate(IEnumerable<IEditable> entities, string connectionString, int timeOuts)
        {
            int affectedRows = 0;
            bool allSucceeded = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {   
                    try
                    {
                        foreach (IEditable entity in entities)
                        {
                            affectedRows += Database.InsertOrUpdate(entity, connection, trans, timeOuts);
                        }

                        trans.Commit();
                        allSucceeded = true;
                    }
                    finally
                    {
                        if (!allSucceeded)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            if (!allSucceeded)
            {
                throw new CrossplException("Couldn't commit to the database");
            } 
            
            return affectedRows;
        }

        #endregion // InsertOrUpdate

        #region Reorder

        public static int Reorder(IEnumerable<IOrderable> entities, bool isForAll, string connectionString)
        {
            return Reorder(entities, isForAll, connectionString, Database.TimeoutSecs);
        }

        public static int Reorder(IEnumerable<IOrderable> entities, bool isForAll, string connectionString, int timeOuts)
        {
            // execute
            int affectedRows = 0;
            bool succeeded = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        affectedRows = Reorder(entities, isForAll, connection, trans, timeOuts);

                        trans.Commit();
                        succeeded = true;
                    }
                    finally
                    {
                        if (!succeeded)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            if (!succeeded)
            {
                throw new CrossplException("Couldn't commit to the database");
            }
            
            return affectedRows;
        }

        public static int Reorder(IEnumerable<IOrderable> entities, bool isForAll, SqlConnection connection, SqlTransaction transaction, int timeOuts)
        {
            string query = string.Empty;
            foreach (IOrderable entity in entities)
            {
                if (isForAll)
                {
                    query += entity.ReorderForAllQuery();
                }
                else
                {
                    query += entity.ReorderQuery();
                }
                query += "\r\n";
            }

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = transaction;
                command.CommandTimeout = timeOuts;
                return command.ExecuteNonQuery();
            }
        }

        #endregion // Reorder

        #region Delete

        public static int Delete(IEditable entity, SqlConnection connection, SqlTransaction transaction, int timeoutSeconds)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = entity.DeleteQuery();
                command.Transaction = transaction;
                command.CommandTimeout = timeoutSeconds;
                entity.AddSqlParameters(command, DBaseOperation.Delete);

                return command.ExecuteNonQuery();
            }
        }

        public static int Delete(IEditable entity, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return Delete(entity, connection, null, Database.TimeoutSecs);
            }
        }

        public static int Delete(IEnumerable<IEditable> entities, string connectionString)
        {
            return Delete(entities, connectionString, Database.TimeoutSecs);
        }

        public static int Delete(IEnumerable<IEditable> entities, string connectionString, int timeOuts)
        {
            int affectedRows = 0;
            bool allSucceeded = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {   
                    try
                    {
                        foreach (IEditable entity in entities)
                        {
                            affectedRows += Database.Delete(entity, connection, trans, timeOuts);
                        }

                        trans.Commit();
                        allSucceeded = true;
                    }
                    finally
                    {
                        if (!allSucceeded)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            if (!allSucceeded)
            {
                throw new CrossplException("Couldn't commit to the database");
            }

            return affectedRows;
        }

        #endregion // Delete

        #region General Purpose

        public static List<T> ExecSProc<T>(string connString, string sproc, params SqlParameter[] parameters) where T : IDBEntity, new()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                return ExecSProc<T>(conn, null, sproc, parameters);
            }
        }

        public static List<T> ExecSProc<T>(SqlConnection conn, SqlTransaction trans, string sproc, params SqlParameter[] parameters) where T : IDBEntity, new()
        {
            List<T> list = new List<T>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sproc;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = new T();
                        item.InitFromSqlReader(reader);
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        // Example Lambda parameter for long: (reader, index) => { return reader.GetInt64(index); },
        public static List<T> ExecSProcPrimitive<T>(SqlConnection conn, SqlTransaction trans, string sproc, SqlTypeConverter<T> converter, params SqlParameter[] parameters)
        {
            List<T> list = new List<T>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sproc;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = default(T);
                        item = converter(reader, 0);
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public static T ExecScalar<T>(string connString, string sproc, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                return ExecScalar<T>(conn, null, sproc, parameters);
            }
        }

        public static T ExecScalar<T>(SqlConnection conn, SqlTransaction trans, string sproc, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sproc;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                object o = cmd.ExecuteScalar();
                if (o != null)
                {
                    return (T)o;
                }
            }

            return default(T);
        }

        public static int ExecNonQuery(string connString, string sproc, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                return ExecNonQuery(conn, null, sproc, parameters);
            }
        }

        public static int ExecNonQuery(SqlConnection conn, SqlTransaction trans, string sproc, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sproc;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }
 
        #endregion

        public static string ShortenQuery(string query)
        {
            query = query.Replace("\t", " ").Replace("\r\n", " ").Replace("\n", " ");
            while (query.Contains("  "))
            {
                query = query.Replace("  ", " ");
            }

            return query;
        }

        public static SqlParameter SqlParam(string name, object value)
        {
            if (value != null)
            {
                if (value is DateTime)
                {
                    if ((DateTime)value == default(DateTime))
                    {
                        return new SqlParameter(name, DBNull.Value);
                    }
                }

                return new SqlParameter(name, value);
            }
            else
            {
                return new SqlParameter(name, DBNull.Value);
            }
        }

        public static void AddSqlParamOrNull(SqlCommand command, string name, object value)
        {
            command.Parameters.Add(SqlParam(name, value));
        }

        public static double GetDoubleOrDefault(SqlDataReader reader, ref int index)
        {
            double val = 0.0;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetDouble(index);
            }
            index++;
            return val;
        }

        public static float GetFloatOrDefault(SqlDataReader reader, ref int index)
        {
            float val = default(float);
            if (!reader.IsDBNull(index))
            {
                val = reader.GetFloat(index);
            }
            index++;
            return val;
        }

        public static decimal GetDecimalOrDefault(SqlDataReader reader, ref int index)
        {
            decimal val = default(decimal);
            if (!reader.IsDBNull(index))
            {
                val = reader.GetDecimal(index);
            }
            index++;
            return val;
        }

        public static long GetInt64OrDefault(SqlDataReader reader, ref int index)
        {
            long val = 0;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetInt64(index);
            }
            index++;
            return val;
        }

        public static int GetInt32OrDefault(SqlDataReader reader, ref int index)
        {
            int val = 0;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetInt32(index);
            }
            index++;
            return val;
        }

        public static short GetShortOrDefault(SqlDataReader reader, ref int index)
        {
            short val = 0;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetInt16(index);
            }
            index++;
            return val;
        }

        public static byte GetByteOrDefault(SqlDataReader reader, ref int index)
        {
            byte val = 0;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetByte(index);
            }
            index++;
            return val;
        }

        public static bool GetBoolOrDefault(SqlDataReader reader, ref int index)
        {
            bool val = false;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetBoolean(index);
            }
            index++;
            return val;
        }

        public static string GetStringOrDefault(SqlDataReader reader, ref int index)
        {
            string val = null;
            if (!reader.IsDBNull(index))
            {
                val = reader.GetString(index);
            }
            index++;
            return val;
        }

        public static DateTime GetDateTimeOrDefault(SqlDataReader reader, ref int index)
        {
            DateTime val = default(DateTime);
            if (!reader.IsDBNull(index))
            {
                val = reader.GetDateTime(index);
            }
            index++;
            return val;
        }
    }
}

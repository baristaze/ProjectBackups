using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace YummyZone.ObjectModel
{
    public class Database
    {
        public const int TimeoutSecs = 30;
        public const int TimeoutSecsForFile = 60;

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

        public static L SelectAll<T, L>(string connectionString, Guid groupId) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            return SelectAll<T, L>(connectionString, groupId, Database.TimeoutSecs);
        }

        public static L SelectAll<T, L>(string connectionString, Guid groupId, int timeoutSeconds) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return SelectAll<T, L>(connection, null, groupId, timeoutSeconds);
            }
        }

        public static L SelectAll<T, L>(SqlConnection connection, SqlTransaction transaction, Guid groupId, int timeoutSeconds) 
            where T : IEditable, new()
            where L : List<T>, new()
        {
            string query = (new T()).SelectAllQuery(groupId);
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
                throw new YummyZoneException("Couldn't commit to the database");
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
                throw new YummyZoneException("Couldn't commit to the database");
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
                throw new YummyZoneException("Couldn't commit to the database");
            }

            return affectedRows;
        }

        #endregion // Delete

        public static string ShortenQuery(string query)
        {
            query = query.Replace("\t", " ").Replace("\r\n", " ").Replace("\n", " ");
            while (query.Contains("  "))
            {
                query = query.Replace("  ", " ");
            }

            return query;
        }
    }
}

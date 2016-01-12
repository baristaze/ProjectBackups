using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public abstract class LoggerBase
    {
        protected enum LogType
        {
            Verbose,
            Info,
            Important,
            Error,
            Exception
        }

        protected enum AppType
        {
            MobileSvc,
            ImageWebSite,
            AdminWebSite
        }

        protected abstract string GetLogStoreConnectionString();

        protected abstract AppType GetAppType();

        public void WriteVerbose(string methodName, Guid contextId, string format, params object[] parameters)
        {
            Write(LogType.Verbose, methodName, contextId, format, parameters);
        }

        public void WriteInfo(string methodName, Guid contextId, string format, params object[] parameters)
        {
            Write(LogType.Info, methodName, contextId, format, parameters);
        }

        public void WriteImportant(string methodName, Guid contextId, string format, params object[] parameters)
        {
            Write(LogType.Important, methodName, contextId, format, parameters);
        }

        public void WriteError(string methodName, Guid contextId, string format, params object[] parameters)
        {
            Write(LogType.Error, methodName, contextId, format, parameters);
        }

        public void WriteException(Exception ex, string methodName, Guid contextId)
        {
            Write(LogType.Exception, methodName, contextId, ex.ToString());
        }

        protected void Write(LogType logType, string methodName, Guid contextId, string format, params object[] parameters)
        {
            try
            {
                string message = String.Format(CultureInfo.InvariantCulture, format, parameters);
                _Write(logType, methodName, contextId, message);
            }
            catch
            {
            }
        }

        protected void _Write(LogType logType, string methodName, Guid contextId, string message)
        {
            string query = "INSERT INTO [dbo].[Log]([App], [Type], [Operation], [ContextId], [Message]) " +
                            "VALUES (@App, @Type, @Operation, @ContextId, @Message)";

            using (SqlConnection connection = new SqlConnection(this.GetLogStoreConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandTimeout = 5; // seconds

                    command.Parameters.Add(new SqlParameter("@App", (byte)this.GetAppType()));
                    command.Parameters.Add(new SqlParameter("@Type", (byte)logType));
                    command.Parameters.Add(new SqlParameter("@Operation", methodName));
                    command.Parameters.Add(new SqlParameter("@ContextId", contextId == Guid.Empty ? DBNull.Value : (object)contextId));
                    command.Parameters.Add(new SqlParameter("@Message", String.IsNullOrWhiteSpace(message) ? DBNull.Value : (object)message));

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

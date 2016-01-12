    using System;
    using System.Globalization;
    using System.Data.SqlClient;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YummyZone.ObjectModel;

    namespace YummyZone.Image.Web
    {
        public class Logger : LoggerBase
        {
            protected override LoggerBase.AppType GetAppType()
            {
                return AppType.ImageWebSite;
            }

            protected override string GetLogStoreConnectionString()
            {
                return File.ConnectionString;
            }

            public static void LogAsVerbose(string methodName, Guid contextId, string format, params object[] parameters)
            {
                // (new Logger()).WriteVerbose(methodName, contextId, format, parameters);
            }

            public static void LogAsInfo(string methodName, Guid contextId, string format, params object[] parameters)
            {
                (new Logger()).WriteInfo(methodName, contextId, format, parameters);
            }

            public static void LogAsImportant(string methodName, Guid contextId, string format, params object[] parameters)
            {
                (new Logger()).WriteImportant(methodName, contextId, format, parameters);
            }

            public static void LogAsError(string methodName, Guid contextId, string format, params object[] parameters)
            {
                (new Logger()).WriteError(methodName, contextId, format, parameters);
            }

            public static void LogException(Exception ex, string methodName, Guid contextId)
            {
                (new Logger()).WriteException(ex, methodName, contextId);
            }
        }
    }
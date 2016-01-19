using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Services.Client;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Crosspl.ObjectModel
{
    public class WADLogsTable : TableServiceEntity
    {
        public static int MaxRecordCount = 10000;
        public const string TableName = "WADLogsTable";

        public Int64 EventTickCount { get; set; }
        public String DeploymentId { get; set; }
        public String Role { get; set; }
        public String RoleInstance { get; set; }
        public Int32 Level { get; set; }
        public Int32 EventId { get; set; }
        public Int32 Pid { get; set; }
        public Int32 Tid { get; set; }
        public String Message { get; set; }

        private static TableServiceContext GetTableServiceContext(string connectionString)
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get the data service context
            TableServiceContext serviceContext = tableClient.GetDataServiceContext();

            return serviceContext;
        }

        public static int DeleteAll(string connectionString)
        {
            int totalDeleteCount = 0;
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);
            while (true)
            {
                int chunk = 100;
                CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).Take(chunk).AsTableServiceQuery<WADLogsTable>();

                int deleteCount = Delete(serviceContext, allElementsQuery);
                totalDeleteCount += deleteCount;

                if (deleteCount <= 0)
                {
                    break;
                }
            }

            Console.WriteLine("{0} log items have been deleted.", totalDeleteCount);
            return totalDeleteCount;
        }

        public static int Delete(string connectionString, int oldestX)
        {
            int totalDeleteCount = 0;
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);
            while (totalDeleteCount < oldestX)
            {
                int chunk = Math.Min(100, oldestX - totalDeleteCount);
                CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).Take(chunk).AsTableServiceQuery<WADLogsTable>();

                int deleteCount = Delete(serviceContext, allElementsQuery);
                totalDeleteCount += deleteCount;

                if (deleteCount <= 0)
                {
                    break;
                }
            }

            Console.WriteLine("{0} log items have been deleted.", totalDeleteCount);
            return totalDeleteCount;
        }

        public static int Delete(string connectionString, DateTime startUTC, DateTime endUTC)
        {
            int totalDeleteCount = 0;
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);
            while (true)
            {
                int chunk = 100;
                CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     where e.Timestamp >= startUTC
                     where e.Timestamp < endUTC 
                     select e).Take(chunk).AsTableServiceQuery<WADLogsTable>();

                int deleteCount = Delete(serviceContext, allElementsQuery);
                totalDeleteCount += deleteCount;

                if (deleteCount <= 0)
                {
                    break;
                }
            }

            Console.WriteLine("{0} log items have been deleted.", totalDeleteCount);
            return totalDeleteCount;
        }

        protected static int Delete(TableServiceContext serviceContext, CloudTableQuery<WADLogsTable> allElementsQuery)
        {
            // Loop through the results
            int deletedCount = 0;
            foreach (TableServiceEntity entity in allElementsQuery)
            {
                Console.WriteLine("Deleting log traced at T={0}", entity.Timestamp);

                // Delete the entity
                serviceContext.DeleteObject(entity);
                deletedCount++;
            }

            // Submit the operation to the table service
            if (deletedCount > 0)
            {
                Console.WriteLine("Committing delete for the next {0} rows", deletedCount);
                serviceContext.SaveChangesWithRetries();
            }

            return deletedCount;
        }

        public static void Print(IEnumerable<WADLogsTable> logs, bool convertTimesToLocal)
        {
            Print(logs, convertTimesToLocal, 1);
        }

        public static void Print(IEnumerable<WADLogsTable> logs, bool convertTimesToLocal, int rowSeparatorCount)
        {
            int counter = 0;
            foreach (WADLogsTable log in logs)
            {
                if (convertTimesToLocal)
                {
                    Console.WriteLine("{0} - {1} PST - {2}", (++counter).ToString("0000"), log.Timestamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), log.Message);
                }
                else
                {
                    Console.WriteLine("{0} - {1} GMT - {2}", (++counter).ToString("0000"), log.Timestamp.ToString("yyyy/MM/dd HH:mm:ss"), log.Message);
                }

                int separator = rowSeparatorCount;
                while (separator > 1)
                {
                    Console.WriteLine();
                    separator--;
                }
            }
        }

        public static IEnumerable<WADLogsTable> GetAll(string connectionString)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);
            
            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).AsTableServiceQuery<WADLogsTable>();

            return allElementsQuery.ToList<WADLogsTable>();
        }

        public static IEnumerable<WADLogsTable> GetAll(string connectionString, string logCategory)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).AsTableServiceQuery<WADLogsTable>();

            return Get(allElementsQuery, logCategory);
        }
        
        public static IEnumerable<WADLogsTable> Get(string connectionString, int oldestX)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).Take(oldestX).AsTableServiceQuery<WADLogsTable>();

            return allElementsQuery.ToList<WADLogsTable>();
        }

        public static IEnumerable<WADLogsTable> Get(string connectionString, int oldestX, string logCategory)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).Take(oldestX).AsTableServiceQuery<WADLogsTable>();

            return Get(allElementsQuery, logCategory);
        }

        public static IEnumerable<WADLogsTable> Get(string connectionString, DateTime startUTC, DateTime endUTC)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     where e.Timestamp >= startUTC
                     where e.Timestamp < endUTC
                     select e).AsTableServiceQuery<WADLogsTable>();

            return allElementsQuery.ToList<WADLogsTable>();
        }

        public static IEnumerable<WADLogsTable> Get(string connectionString, DateTime startUTC, DateTime endUTC, string logCategory)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     where e.Timestamp >= startUTC
                     where e.Timestamp < endUTC
                     select e).AsTableServiceQuery<WADLogsTable>();

            return Get(allElementsQuery, logCategory);
        }

        protected static List<WADLogsTable> Get(CloudTableQuery<WADLogsTable> logs, string logCategory)
        {
            List<WADLogsTable> list = new List<WADLogsTable>();
            foreach (WADLogsTable log in logs)
            {
                if (String.IsNullOrWhiteSpace(logCategory))
                {
                    list.Add(log);
                }
                else if (log.Message.StartsWith(logCategory))
                {
                    list.Add(log);
                }
            }

            return list;
        }

        public static IEnumerable<WADLogsTable> GetBySearchString(string connectionString, DateTime startUTC, DateTime endUTC, params string[] phrases)
        {
            TableServiceContext serviceContext = GetTableServiceContext(connectionString);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     where e.Timestamp >= startUTC
                     where e.Timestamp < endUTC
                     select e).AsTableServiceQuery<WADLogsTable>();

            return GetBySearchString(allElementsQuery, phrases);
        }

        protected static List<WADLogsTable> GetBySearchString(CloudTableQuery<WADLogsTable> logs, params string[] phrases)
        {
            List<WADLogsTable> list = new List<WADLogsTable>();
            foreach (WADLogsTable log in logs)
            {
                if (ContainsPhrase(log.Message, phrases))
                {
                    list.Add(log);
                }
            }

            return list;
        }

        private static bool ContainsPhrase(string bulk, params string[] phrases)
        {
            if (String.IsNullOrWhiteSpace(bulk))
            {
                return false;
            }

            if (phrases == null)
            {
                return false;
            }

            foreach (string s in phrases)
            {
                if (bulk.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

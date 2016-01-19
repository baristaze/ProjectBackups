using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Pic4Pic.ObjectModel
{
    public class LogProcessor : BaseTask<LogProcessorConfig>
    {
        public const string TableName = "WADLogsTable";

        public LogProcessor(LogProcessorConfig config)
        {
            this.Config = config;
        }

        public void Run()
        {
            Logger.bag().LogInfo("LogProcessor is starting");
            Logger.bag().LogInfo(this.Config.ToString());

            while (true)
            {
                if (!this.Init(this.Config.TaskConfigMeta))
                {
                    Logger.bag().LogError("LogProcessor couldn't be initialized");
                }

                // run either new or old config
                this.PerformTask(this.Config);
            }
        }

        public override void PerformTask(LogProcessorConfig config)
        {
            TableServiceContext serviceContext = this.GetTableServiceContext(config);

            CloudTableQuery<WADLogsTable> allElementsQuery =
                    (from e in serviceContext.CreateQuery<WADLogsTable>(TableName)
                     select e).Take(config.ChunkSizeForLogRead).AsTableServiceQuery<WADLogsTable>();

            List<WADLogsTable> logRows = new List<WADLogsTable>();
            foreach (WADLogsTable logRow in allElementsQuery)
            {
                logRows.Add(logRow);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(config.LogDBaseConnString))
                {
                    conn.Open();
                    this.Process(conn, logRows);
                }
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError("Processing of Azure Logs have failed");
            }

            try
            {
                this.Delete(serviceContext, logRows);
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError("Deleting of Azure Logs have failed");
            }
        }

        protected int Process(SqlConnection conn, List<WADLogsTable> logRows)
        {
            int savedCount = 0;
            foreach (WADLogsTable logRow in logRows)
            {
                // get log bag
                LogBag logBag = LogBag.Parse(logRow.Message);

                // check the log bag
                if (logBag == null)
                {
                    try
                    {
                        this.SaveUnProcessed(conn, logRow);
                    }
                    catch (Exception ex)
                    {
                        Logger.bag().Add(ex).LogError("Saving an unprocessed Azure Log has failed");
                    }
                    
                    continue;
                }

                // check log ID as it is a must
                if (logBag.GetByTag(LogBag.TagLogId) == null)
                {
                    logBag.Add(LogBag.TagLogId, Guid.NewGuid().ToString());
                }

                // check log time as it is a must
                if (logBag.GetByTag(LogBag.TagLogTime) == null)
                {
                    logBag.Add(LogBag.TagLogTime, logRow.Timestamp.ToString(
                        "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                }

                // save to db
                try
                {
                    this.SaveProcessed(conn, logBag, logRow.Message, logRow.Timestamp);
                }
                catch (Exception e)
                {
                    Logger.bag().Add(e).LogError("Saving a processed Azure Log has failed");

                    try
                    {
                        this.SaveUnProcessed(conn, logRow);
                    }
                    catch (Exception ex)
                    {
                        Logger.bag().Add(ex).LogError("Saving an unprocessed Azure Log has failed");
                    }
                }
                
            }

            return savedCount;
        }

        protected int SaveProcessed(SqlConnection conn, LogBag logBag, String fullMessage, DateTime timeStamp)
        {
            DateTime time = logBag.GetDateTimeByTag("LogTimeUTC");
            if (time == default(DateTime))
            {
                time = timeStamp;
            }

            return Database.ExecNonQuery(
                conn,
                null,
                "[dbo].[SaveProcessedAzureLog]",
                Database.SqlParam("@LogId", logBag.GetGuidByTag("LogId")),                  // + guid
                Database.SqlParam("@TimeUTC", time),                                        // + datetime
                Database.SqlParam("@ClientId", logBag.GetGuidByTag("ClientId")),            // + guid
                Database.SqlParam("@UserId", logBag.GetGuidByTag("UserId")),                // + guid
                Database.SqlParam("@SplitId", logBag.GetIntByTag("Split", 0)),              // + int
                Database.SqlParam("@Level", logBag.GetByTag("Level")),                      // +
                Database.SqlParam("@Platform", logBag.GetByTag("AppType")),                 // +
                Database.SqlParam("@Version", logBag.GetByTag("AppVersion")),               // +
                Database.SqlParam("@File", logBag.GetByTag("File")),                        // +
                Database.SqlParam("@Class", logBag.GetByTag("Class")),                      // +
                Database.SqlParam("@Method", logBag.GetByTag("Method")),                    // +
                Database.SqlParam("@Line", logBag.GetIntByTag("Line", 0)),                  // + int
                Database.SqlParam("@Message", logBag.GetByTag("Message")),                  // +
                Database.SqlParam("@Exception", logBag.GetByTag("Exception")),              // +
                Database.SqlParam("@ElapsedTimeMSec", logBag.GetIntByTag("ElapsedTimeMSec", 0)),                  // + int
                Database.SqlParam("@Funnel", logBag.GetByTag("Funnel")),                    // +
                Database.SqlParam("@Page", logBag.GetByTag("Page")),                        // +
                Database.SqlParam("@Action", logBag.GetByTag("Action")),                    // +
                Database.SqlParam("@Step", logBag.GetByTag("Step")),                        // +
                Database.SqlParam("@Full", fullMessage));                                   // 
        }

        protected int SaveUnProcessed(SqlConnection conn, WADLogsTable row)
        {
            return Database.ExecNonQuery(
                conn,
                null,
                "[dbo].[SaveUnprocessedAzureLog]",
                Database.SqlParam("@RowKey", row.RowKey),
                Database.SqlParam("@TimeUTC", row.Timestamp),
                Database.SqlParam("@Message", row.Message));
        }

        protected int Delete(TableServiceContext serviceContext, List<WADLogsTable> logRows)
        {
            // Loop through the results
            int deletedCount = 0;
            foreach (WADLogsTable entity in logRows)
            {
                // Delete the entity
                serviceContext.DeleteObject(entity);
                deletedCount++;
            }

            // Submit the operation to the table service
            if (deletedCount > 0)
            {
                serviceContext.SaveChangesWithRetries();
            }

            return deletedCount;
        }

        private TableServiceContext GetTableServiceContext(LogProcessorConfig config)
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.AzureLogTableConnString);

            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get the data service context
            TableServiceContext serviceContext = tableClient.GetDataServiceContext();

            return serviceContext;
        }
    }
}

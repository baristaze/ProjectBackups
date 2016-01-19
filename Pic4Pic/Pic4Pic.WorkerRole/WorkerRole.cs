using System;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private const int ANDROID_PUSHER = 1;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            System.Diagnostics.Trace.TraceInformation("Pic4Pic.WorkerRole entry point called");

            try
            {
                this._Run();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

        protected void _Run() 
        {
            PushNotificationWorker worker1 = new PushNotificationWorker();
            Thread workerThread1 = new Thread(worker1.Run);

            LogParsingWorker worker2 = new LogParsingWorker();
            Thread workerThread2 = new Thread(worker2.Run);
            
            workerThread1.Start();
            workerThread2.Start();

            workerThread1.Join();
            workerThread2.Join();
        }

        public override bool OnStart()
        {
            Logger.AppType = "Worker";

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Transfer logs to storage every minute
            TimeSpan pushInterval = TimeSpan.FromSeconds(30);
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = pushInterval;
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            string diagStoreConnStrInCfg = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            DiagnosticMonitor.Start(diagStoreConnStrInCfg, dmc);

            return base.OnStart();
        }
    }
}

using System;
using System.Net;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Crosspl.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            // Transfer logs to storage every minute
            TimeSpan pushInterval = TimeSpan.FromMinutes(1);
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.ScheduledTransferPeriod = pushInterval;
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            string diagStoreConnStrInCfg = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            DiagnosticMonitor.Start(diagStoreConnStrInCfg, dmc);

            return base.OnStart();
        }
    }
}

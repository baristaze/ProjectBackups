using System;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WorkerRole
{
    public class LogParsingWorker
    {
        private const int LOG_PARSER = 2;

        public void Run()
        {
            try
            {
                this._Run();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("LogParsingWorker failed to start. Exception: " + ex.ToString());
            }
        }

        protected void _Run()
        {
            // create basic config            
            Config config = new Config();
            if (!config.Init())
            {
                System.Diagnostics.Trace.TraceError("LogParsingWorker core config couldn't be initialized.");
                return;
            }

            // create meta config
            TaskConfigMeta configMeta = new TaskConfigMeta();
            configMeta.ConfigDBaseConnStr = config.DBaseConnectionString;
            configMeta.ConfigRefreshPeriodInSec = config.ConfigRefreshPeriodInSec;
            configMeta.EnvironmentId = (byte)config.EnvironmentId;
            configMeta.ProjectId = LOG_PARSER;

            LogProcessorConfig agentConfig = new LogProcessorConfig();
            agentConfig.TaskConfigMeta = configMeta;

            // create and init the agent
            LogProcessor agent = new LogProcessor(agentConfig);
            if (!agent.Init(configMeta))
            {
                // this initial Init is for sanity-check only...
                System.Diagnostics.Trace.TraceError("LogProcessor agent config couldn't be initialized.");
                return;
            }

            // run the agent
            agent.Run();
        }
    }
}

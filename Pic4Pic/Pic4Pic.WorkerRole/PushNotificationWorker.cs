using System;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WorkerRole
{
    public class PushNotificationWorker
    {
        private const int ANDROID_PUSHER = 1;

        public void Run()
        {
            try
            {
                this._Run();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("PushNotificationWorker failed to start. Exception: " + ex.ToString());
            }
        }

        protected void _Run()
        {
            // create basic config            
            Config config = new Config();
            if (!config.Init())
            {
                System.Diagnostics.Trace.TraceError("PushNotificationWorker core config couldn't be initialized.");
                return;
            }

            // create meta config
            TaskConfigMeta configMeta = new TaskConfigMeta();
            configMeta.ConfigDBaseConnStr = config.DBaseConnectionString;
            configMeta.ConfigRefreshPeriodInSec = config.ConfigRefreshPeriodInSec;
            configMeta.EnvironmentId = (byte)config.EnvironmentId;
            configMeta.ProjectId = ANDROID_PUSHER;

            AndroidNotificationPusherConfig agentConfig = new AndroidNotificationPusherConfig();
            agentConfig.TaskConfigMeta = configMeta;

            // create and init the agent
            AndroidNotificationPusher agent = new AndroidNotificationPusher(agentConfig);
            if (!agent.Init(configMeta))
            {
                // this initial Init is for sanity-check only...
                System.Diagnostics.Trace.TraceError("AndroidNotificationPusher agent config couldn't be initialized.");
                return;
            }

            // run the agent
            agent.Run();
        }
    }
}

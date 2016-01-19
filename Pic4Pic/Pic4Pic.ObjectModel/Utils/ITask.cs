using System;
using System.Diagnostics;

namespace Pic4Pic.ObjectModel
{
    public class TaskConfigMeta
    {
        public string ConfigDBaseConnStr { get; set; }
        public byte ProjectId { get; set; }
        public byte EnvironmentId { get; set; }
        public int ConfigRefreshPeriodInSec { get; set; }
    }

    public interface ITask
    {
        bool Init(TaskConfigMeta taskConfigMeta);
        void Perform();
    }

    public abstract class AbstractTask : ITask
    {
        public virtual bool Init(TaskConfigMeta taskConfigMeta)
        {
            return true;
        }

        public abstract void Perform();
    }

    public abstract class BaseTask<CONFIG> : AbstractTask where CONFIG : ConfigOnDBase, new()
    {
        protected DateTime lastSuccessfullInit = DateTime.Now.AddDays(-1);

        public CONFIG Config { get; protected set; }

        public override bool Init(TaskConfigMeta taskConfigMeta)
        {
            bool reinit = true;
            if (this.Config != null)
            {
                TimeSpan diff = DateTime.Now - lastSuccessfullInit;
                if (diff.TotalSeconds < taskConfigMeta.ConfigRefreshPeriodInSec)
                {
                    reinit = false;
                }
            }

            if (reinit)
            {
                CONFIG cfg = new CONFIG();
                cfg.TaskConfigMeta = taskConfigMeta;
                if (cfg.Init())
                {
                    this.Config = cfg;
                    lastSuccessfullInit = DateTime.Now;
                    Logger.bag().LogInfo("Refreshing config for " + this.GetType().Name);
                }
            }

            return this.Config != null;
        }

        public override void Perform()
        {
            this.PerformTask(this.Config);
        }

        public abstract void PerformTask(CONFIG config);
    }
}

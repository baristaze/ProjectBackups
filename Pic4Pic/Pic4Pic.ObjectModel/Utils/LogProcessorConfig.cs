using System;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Pic4Pic.ObjectModel
{
    public class LogProcessorConfig : ConfigOnDBase
    {
        public string AzureLogTableConnString { get; set; }

        public string LogDBaseConnString { get; set; }

        public int ChunkSizeForLogRead { get; set; }

        public LogProcessorConfig()
        { 
        }

        public override bool Init()
        {
            if (!base.Init())
            {
                return false;
            }

            this.AzureLogTableConnString = this.Get("AzureLogTableConnString");
            this.LogDBaseConnString = this.Get("LogDBaseConnString");
            this.ChunkSizeForLogRead = this.GetAsInt("ChunkSizeForLogRead", 100, 10, 3000);

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "AzureLogTableConnString", this.AzureLogTableConnString);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "LogDBaseConnString", this.LogDBaseConnString);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "ChunkSizeForLogRead", this.ChunkSizeForLogRead);
            str.AppendLine();
            str.AppendLine();

            
            return str.ToString();
        }
    }
}

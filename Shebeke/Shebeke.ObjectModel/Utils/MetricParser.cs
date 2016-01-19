using System;

namespace Shebeke.ObjectModel
{
    public abstract class MetricParser<T> : LogParser<T> where T : IPrintable
    {   
        public const string MetricPreamble = "Metric: ";

        public string ConsumePreamble(string message)
        {
            return this.ConsumePreamble(message, MetricPreamble);
        }

        public virtual string ConsumeVersion(string message, out int version)
        {
            version = 0;
            NameValuePair<int> pair = null;
            message = this.ParseNext(message, out pair, 1, 10);
            if (pair != null)
            {
                version = pair.Value;                
            }

            // throw new ShebekeException("Log message doesn't have version info");
            return message;
        }

        public virtual string ConsumeMetricName(string message, out string metricName)
        {
            NameValuePair<string> pair = null;
            message = this.ParseNext(message, out pair);
            if (pair != null)
            {
                metricName = pair.Value;
                return message;
            }

            throw new ShebekeException("Log message doesn't have a metric name");
        }
    }
}

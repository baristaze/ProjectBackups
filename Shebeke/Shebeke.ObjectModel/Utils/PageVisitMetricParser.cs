using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class PageVisitMetricParser : MetricParser<PageVisit>
    {
        public override PageVisit Parse(WADLogsTable log)
        {
            if (log == null || String.IsNullOrWhiteSpace(log.Message))
            {
                return null;
            }

            if (!log.Message.StartsWith(MetricParser<PageVisit>.MetricPreamble))
            {
                return null;
            }

            string message = this.ConsumePreamble(log.Message);

            int version = 0;
            message = this.ConsumeVersion(message, out version);
            if (version <= 0) 
            {
                return null;
            }

            string metricName = null;
            message = this.ConsumeMetricName(message, out metricName);
            if (metricName != "PageVisit")
            {   
                return null;
            }

            NameValuePair<string> pairStr = null;
            NameValuePair<long> pairLong = null;

            PageVisit visit = new PageVisit();
            visit.ShareTimeUTC = log.Timestamp;

            pairStr = null;
            message = this.ParseNextAndValidateName(message, out pairStr, "Page");
            visit.Page = pairStr.Value;

            pairStr = null;
            message = this.ParseNextAndValidateName(message, out pairStr, "Referrer");
            visit.Referrer = pairStr.Value;

            pairLong = null;
            message = this.ParseNextAndValidateName(message, out pairLong, "UserId", -1, long.MaxValue);
            visit.UserId = pairLong.Value;

            pairLong = null;
            message = this.ParseNextAndValidateName(message, out pairLong, "TopicId", -1, long.MaxValue);
            visit.TopicId = pairLong.Value;

            pairLong = null;
            message = this.ParseNextAndValidateName(message, out pairLong, "EntryId", -1, long.MaxValue);
            visit.EntryId = pairLong.Value;

            NameValuePair<int> pairInt = null;
            if (version > 1)
            {
                message = this.ParseNextAndValidateName(message, out pairInt, "Split", 0, int.MaxValue);
                visit.Split = pairInt.Value;
            }

            return visit;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public partial class Work : IDBEntity
    {
        public NameLongIdPair Employer { get; set; }
        public NameLongIdPair Position { get; set; }
        public DateTime StartDate;
        public DateTime EndDate;

        public bool IsCurrent()
        {
            return this.EndDate == default(DateTime);
        }

        public override string ToString()
        {
            string s = String.Empty;
            if (this.Employer != null && !String.IsNullOrWhiteSpace(this.Employer.Name)) 
            {
                s += this.Employer.Name;
            }

            if (this.Position != null && !String.IsNullOrWhiteSpace(this.Position.Name))
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    s += " - ";
                }

                s += this.Position.Name;
            }

            return s;
        }

        public static int ComparisonByTime(Work first, Work second)
        {
            if(first.IsCurrent() && !second.IsCurrent())
            {
                // current job is greater than previous job
                return 1;
            }
            else if(!first.IsCurrent() && second.IsCurrent())
            {
                // current job is greater than previous job
                return -1;
            }
            else if (!first.IsCurrent() && !second.IsCurrent())
            {
                // both are old...
                // in that case, the one that has been left more recent is bigger
                return first.EndDate.CompareTo(second.EndDate);
            }
            else // if (first.IsCurrent() && second.IsCurrent())
            {
                // both are current...
                // in that case, the one that has been started earlier is bigger
                return -1 * first.StartDate.CompareTo(second.StartDate);
            }            
        }

        public static Work CreateFromFacebookWork(dynamic jsonWork)
        {
            if (jsonWork == null || jsonWork.position == null || String.IsNullOrWhiteSpace(jsonWork.position.name))
            {
                return null;
            }
            
            Work work = new Work();
            work.Position = FacebookHelpers.ConvertToNameIdPair(jsonWork.position);
            work.Employer = FacebookHelpers.ConvertToNameIdPair(jsonWork.employer); // it checks null

            if (jsonWork.start_date != null && !String.IsNullOrWhiteSpace(jsonWork.start_date.ToString()))
            {
                string dt = jsonWork.start_date.ToString();
                if (!dt.StartsWith("0000"))
                {
                    try
                    {
                        work.StartDate = DateTime.Parse(dt);
                    }
                    catch (Exception ex)
                    {
                        Logger.bag().Add(ex).LogError("Work start date text couldn't be parsed into DateTime: " + jsonWork.start_date.ToString());
                    }
                }
                
            }

            if (jsonWork.end_date != null && !String.IsNullOrWhiteSpace(jsonWork.end_date.ToString()))
            {
                string dt = jsonWork.end_date.ToString();
                if (!dt.StartsWith("0000")) {
                    try
                    {
                        work.EndDate = DateTime.Parse(dt);
                    }
                    catch (Exception ex)
                    {
                        Logger.bag().Add(ex).LogError("Work end date text couldn't be parsed into DateTime: " + jsonWork.start_date.ToString());
                    }
                }
            }

            return work;
        }
    }

    public partial class WorkHistory : List<Work>
    {
        public WorkHistory() : base() { }

        public WorkHistory(List<Work> items) : base(items) { }

        public string SortAndGetLastPositionName()
        {
            return SortAndGetLastPositionName(false);
        }

        public string SortAndGetLastPositionName(bool includeRecentWorks)
        {
            return SortAndGetLastPositionName(includeRecentWorks, 10);
        }

        public string SortAndGetLastPositionName(bool includeRecentWorks, uint ignoreRecentWorkThresholdInDays)
        {
            if (includeRecentWorks) 
            {
                ignoreRecentWorkThresholdInDays = 0;
            }

            return _SortAndGetLastPositionName(ignoreRecentWorkThresholdInDays);
        }

        protected string _SortAndGetLastPositionName(uint ignoreRecentWorkThresholdInDays)
        {
            this.Sort(Work.ComparisonByTime);
            this.Reverse();
            Work last = null;
            foreach (Work work in this)
            {
                if (ignoreRecentWorkThresholdInDays == 0)
                {
                    last = work;
                    break;
                }
                else
                {
                    DateTime cutoffTime = DateTime.Now.AddDays(-1 * ignoreRecentWorkThresholdInDays);
                    if (work.StartDate < cutoffTime)
                    {
                        last = work;
                        break;
                    }
                }
            }

            if (last == null && this.Count > 0)
            {
                last = this[0];
            }

            if (last != null)
            {
                return last.Position.Name;
            }

            return null;
        }

        // from newest to oldest by default. we don't change the order
        public static WorkHistory CreateFromFacebookArray(dynamic jsonWorkArray)
        {
            WorkHistory history = new WorkHistory();
            if (jsonWorkArray == null || jsonWorkArray.Count == 0)
            {
                // return empty
                return history;
            }

            for (int x = 0; x < jsonWorkArray.Count; x++)
            {
                Work work = Work.CreateFromFacebookWork(jsonWorkArray[x]);
                if (work != null)
                {
                    history.Add(work);
                }
            }

            return history;
        }
    }
}

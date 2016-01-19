using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public class LogProcessor
    {
        public static void PrintErrors(string connectionString, int lastXDays)
        {
            lastXDays = Math.Abs(lastXDays) * -1;
            DateTime now = DateTime.UtcNow;
            DateTime past = now.AddDays(lastXDays);

            IEnumerable<WADLogsTable> logs = WADLogsTable.Get(connectionString, past, now, LogCategory.Error);
            WADLogsTable.Print(logs, true, 3);
        }

        public static void PrintAll(string connectionString, DateTime startTimeUTC, DateTime endTimeUTC)
        {
            IEnumerable<WADLogsTable> logs = WADLogsTable.Get(connectionString, startTimeUTC, endTimeUTC, null);
            WADLogsTable.Print(logs, true, 1);
        }

        public static void PrintBySearchString(string connectionString, DateTime startTimeUTC, DateTime endTimeUTC, params string[] searchPhrases)
        {
            IEnumerable<WADLogsTable> logs = WADLogsTable.GetBySearchString(connectionString, startTimeUTC, endTimeUTC, searchPhrases);
            WADLogsTable.Print(logs, true, 1);
        }

        public static List<SocialShare> GetSocialShares(string connectionString, int lastXDays)
        {
            return ReadAndParseLogs<SocialShareMetricParser, SocialShare>(connectionString, lastXDays);
        }

        public static List<PageVisit> GetPageVisits(string connectionString, int lastXDays)
        {
            return ReadAndParseLogs<PageVisitMetricParser, PageVisit>(connectionString, lastXDays);
        }

        public static void WriteSocialSharesToFiles(string connectionString, int lastXDays, string filePathPrefix)
        {
            List<SocialShare> shares = LogProcessor.GetSocialShares(connectionString, lastXDays);

            List<TopicSocialShare> topicShares = new List<TopicSocialShare>();
            List<EntrySocialShare> entryShares = new List<EntrySocialShare>();
            List<ReactionSocialShare> reactionShares = new List<ReactionSocialShare>();
            List<VoteSocialShare> voteShares = new List<VoteSocialShare>();

            foreach (SocialShare share in shares)
            {
                if (share is ReactionSocialShare)
                {
                    reactionShares.Add(share as ReactionSocialShare);
                }
                else if (share is VoteSocialShare)
                {
                    voteShares.Add(share as VoteSocialShare);
                }
                else if (share is EntrySocialShare)
                {
                    entryShares.Add(share as EntrySocialShare);
                }
                else if (share is TopicSocialShare)
                {
                    topicShares.Add(share as TopicSocialShare);
                }
                else
                {
                    throw new CrossplException("");
                }
            }

            WriteToFile(topicShares, filePathPrefix + "_topics.csv");
            WriteToFile(entryShares, filePathPrefix + "_entries.csv");
            WriteToFile(reactionShares, filePathPrefix + "_reactions.csv");
            WriteToFile(voteShares, filePathPrefix + "_votes.csv");
            WriteToFile(shares, filePathPrefix + "_all.csv");
        }

        public static void WritePageVisitsToFiles(string connectionString, int lastXDays, string filePath)
        {
            List<PageVisit> logs = LogProcessor.GetPageVisits(connectionString, lastXDays);
            WriteToFile(logs, filePath);
        }

        protected static void WriteToFile<T>(List<T> items, string filePath) where T : IPrintable, new()
        {
            T template = new T();
            StringBuilder content = new StringBuilder();
            List<string> columns = template.GetPropertyNames();
            content.AppendLine(String.Join(",", columns));
            foreach (T item in items)
            {
                content.AppendLine(item.ToPrintString(","));
            }

            File.WriteAllText(filePath, content.ToString());
        }

        protected static List<ParsedLog> ReadAndParseLogs<Parser, ParsedLog>(string connectionString, int lastXDays)
            where Parser : LogParser<ParsedLog>, new()
            where ParsedLog : IPrintable
        {
            lastXDays = Math.Abs(lastXDays) * -1;
            DateTime now = DateTime.UtcNow;
            DateTime past = now.AddDays(lastXDays);

            List<ParsedLog> items = new List<ParsedLog>();
            Parser parser = new Parser();
            IEnumerable<WADLogsTable> logs = WADLogsTable.Get(connectionString, past, now, LogCategory.Metric);
            foreach (WADLogsTable log in logs)
            {
                ParsedLog item = parser.Parse(log);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}

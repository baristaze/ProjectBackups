using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class Recommendation : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.Id = Database.GetGuidOrDefault(reader, ref index);
            this.UserId1 = Database.GetGuidOrDefault(reader, ref index);
            this.UserId2 = Database.GetGuidOrDefault(reader, ref index);
            this.RecommendTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public static List<FacebookUser> GetRecentRecommendations_FromDBase(
            SqlConnection conn, 
            SqlTransaction trans, 
            Guid userId, 
            int maxCount, 
            int cutOffAsMinutes,
            ref List<DateTime> recommendationUtcTimes)
        {
            // preapare parameters
            SqlParameter[] parameters = new SqlParameter[] { 
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@RecentRecommendationsMinuteLimit", cutOffAsMinutes)
            };

            List<FacebookUser> list = new List<FacebookUser>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "[dbo].[GetRecentRecommendations]";
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colIndex = 0;
                        FacebookUser item = new FacebookUser();
                        item.InitFromSqlReader(reader, ref colIndex);
                        list.Add(item);
                        DateTime recommendTimeUTC = Database.GetDateTimeOrDefault(reader, ref colIndex);
                        recommendationUtcTimes.Add(recommendTimeUTC);
                    }
                }
            }

            return list;
        }

        public static List<FacebookUser> MakeNewRecommendations(
            SqlConnection conn, 
            SqlTransaction trans, 
            Guid userId, 
            int maxCount, 
            int cutOffAsMinutes,
            string homeTownState, 
            string concatenatedCitiesInRange)
        {
            return Database.ExecSProc<FacebookUser>(
                conn,
                trans,
                "[dbo].[MakeNewRecommendations]",
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ExcludeFacebookFriends", true),
                Database.SqlParam("@ExcludeRecentRecommendations", true),
                Database.SqlParam("@RecentRecommendationsMinuteLimit", cutOffAsMinutes),
                Database.SqlParam("@MatchState", true),
                Database.SqlParam("@HomeTownState", homeTownState),
                Database.SqlParam("@ConcatenatedCitiesInRange", concatenatedCitiesInRange),
                Database.SqlParam("@InsertToRecommendations", true));
        }

        public static List<FacebookUser> GetPreviewRecommendations(
           SqlConnection conn,
           SqlTransaction trans,
           int maxCount,
           string homeTownState,
           string concatenatedCitiesInRange)
        {
            return Database.ExecSProc<FacebookUser>(
                conn,
                trans,
                "[dbo].[GetPreviewRecommendations]",
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@HomeTownState", homeTownState),
                Database.SqlParam("@ConcatenatedCitiesInRange", concatenatedCitiesInRange));
        } 
    }
}

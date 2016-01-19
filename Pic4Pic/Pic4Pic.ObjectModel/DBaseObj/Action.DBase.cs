using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class Action : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.Id = Database.GetGuidOrDefault(reader, ref index);
            this.UserId1 = Database.GetGuidOrDefault(reader, ref index);
            this.UserId2 = Database.GetGuidOrDefault(reader, ref index);
            this.ActionType = (ActionType)Database.GetByteOrDefault(reader, ref index);
            this.ActionTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.Status = (ActionStatus)Database.GetByteOrDefault(reader, ref index);
            this.NotifScheduleTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.NotifSentTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.NotifViewTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public static List<Action> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffAsMinutes)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetRecentActions]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ActionTypeFilter", (byte)ActionType.Undefined), // all
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static List<Action> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffAsMinutes, ActionType actionTypeFilter)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetRecentActions]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ActionTypeFilter", (byte)actionTypeFilter),
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static List<Action> ReadAllByMeFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffAsMinutes)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetRecentActionsByMe]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ConcatenatedUserIDs", null),
                Database.SqlParam("@ActionTypeFilter", (byte)ActionType.Undefined), // all                
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static List<Action> ReadAllByMeFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffAsMinutes, ActionType actionTypeFilter)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetRecentActionsByMe]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ConcatenatedUserIDs", null),
                Database.SqlParam("@ActionTypeFilter", (byte)actionTypeFilter),
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static List<Action> ReadAllByMeFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, string concatenatedUserIds, int maxCount, int cutOffAsMinutes, ActionType actionTypeFilter)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetRecentActionsByMe]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ConcatenatedUserIDs", concatenatedUserIds),
                Database.SqlParam("@ActionTypeFilter", (byte)actionTypeFilter),
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static List<Action> GetActionsToBeNotifiedFromDBase(SqlConnection conn, SqlTransaction trans, int maxCount, int cutOffAsMinutes)
        {
            return Database.ExecSProc<Action>(
                conn,
                trans,
                "[dbo].[GetActionsToBeNotified]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }

        public static int MarkAsViewedOnDBase(SqlConnection conn, SqlTransaction trans, Guid currentUserId, Guid actionId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MarkNotificationAsViewed]",
                Database.SqlParam("@ActionId", actionId),
                Database.SqlParam("@UserId2", currentUserId));
        }

        public static int MarkNotificationsAsScheduledOnDBase(SqlConnection conn, SqlTransaction trans, String concatenatedActionIDs, DateTime scheduleTimeUTC)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[ScheduleNotifications]",
                Database.SqlParam("@ConcatenatedActionIDs", concatenatedActionIDs),
                Database.SqlParam("@ScheduleTimeUTC", scheduleTimeUTC));
        }

        public static int MarkNotificationsAsSentOnDBase(SqlConnection conn, SqlTransaction trans, String concatenatedActionIDs)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MarkNotificationsAsSent]",
                Database.SqlParam("@ConcatenatedActionIDs", concatenatedActionIDs));
        }

        public static int MarkNotificationsAsOmittedOnDBase(SqlConnection conn, SqlTransaction trans, String concatenatedActionIDs)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MarkNotificationsAsOmitted]",
                Database.SqlParam("@ConcatenatedActionIDs", concatenatedActionIDs));
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[InsertAction]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@UserId1", this.UserId1),
                Database.SqlParam("@UserId2", this.UserId2),
                Database.SqlParam("@ActionType", (byte)this.ActionType));
        }
    }
}

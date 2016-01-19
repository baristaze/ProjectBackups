using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class InstantMessage : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.Id = Database.GetGuidOrDefault(reader, ref index);
            this.UserId1 = Database.GetGuidOrDefault(reader, ref index);
            this.UserId2 = Database.GetGuidOrDefault(reader, ref index);
            this.Content = Database.GetStringOrDefault(reader, ref index);
            this.SentTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.ReadTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public static Guid CreateOnDBase(SqlConnection conn, SqlTransaction trans, InstantMessage message)
        {
            if (message.Id == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid ID on instant message");
            }

            if (message.UserId1 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (1) on instant message");
            }

            if (message.UserId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (2) on instant message");
            }

            if (String.IsNullOrWhiteSpace(message.Content))
            {
                throw new Pic4PicArgumentException("instant message is empty");
            }

            Guid id = Database.ExecScalar<Guid>(
                conn,
                trans,
                "[dbo].[SaveTextMessage]",
                Database.SqlParam("@Id", message.Id),
                Database.SqlParam("@UserId1", message.UserId1),
                Database.SqlParam("@UserId2", message.UserId2),
                Database.SqlParam("@Content", message.Content));

            if (id == Guid.Empty)
            {
                throw new Pic4PicException("Unexpected error while saving instant message on database");
            }

            return id;
        }

        public static List<InstantMessage> ReadConversationFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId1, Guid userId2, int maxCount, int cutOffMinutes)
        {
            return Database.ExecSProc<InstantMessage>(
                conn,
                trans,
                "[dbo].[GetConversation]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId1", userId1),
                Database.SqlParam("@UserId2", userId2),
                Database.SqlParam("@CutOffMinutes", cutOffMinutes));
        }

        public static int MarkAllAsViewedOnDBase(SqlConnection conn, SqlTransaction trans, Guid senderUserId, Guid receiverUserId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MarkAllTextMessagesAsRead]",
                Database.SqlParam("@UserId1", senderUserId),
                Database.SqlParam("@UserId2", receiverUserId));
        }
    }
}

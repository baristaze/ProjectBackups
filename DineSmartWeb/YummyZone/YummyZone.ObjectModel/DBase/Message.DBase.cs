using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Message : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            string query = @"INSERT INTO [dbo].[Message](
                                [GroupId]
                               ,[Id]
                               ,[SenderId]
                               ,[ChainId]
                               ,[ReceiverId]
                               ,[CheckInId]
                               ,[Title]
                               ,[Content]
                               ,[QueueTimeUTC]
                               ,[PushTimeUTC]
                               ,[ReadTimeUTC]
                               ,[DeleteTimeUTC])
                         VALUES(
                                @GroupId
                               ,@Id
                               ,@SenderId
                               ,@ChainId
                               ,@ReceiverId
                               ,@CheckInId
                               ,@Title
                               ,@Content
                               ,@QueueTimeUTC
                               ,@PushTimeUTC
                               ,@ReadTimeUTC
                               ,@DeleteTimeUTC)";

            return Database.ShortenQuery(query);
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            this.Validate();

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@Id", this.Id);
            command.Parameters.AddWithValue("@SenderId", this.SenderId);
            command.Parameters.AddWithValue("@ChainId", this.ChainId);
            command.Parameters.AddWithValue("@ReceiverId", this.ReceiverId);
            command.Parameters.AddWithValue("@CheckInId", this.CheckInId.HasValue ? (object)this.CheckInId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Title", this.Title);
            command.Parameters.AddWithValue("@Content", this.Content);
            command.Parameters.AddWithValue("@QueueTimeUTC", this.QueueTimeUTC);
            command.Parameters.AddWithValue("@PushTimeUTC", this.PushTimeUTC.HasValue ? (object)this.PushTimeUTC.Value : DBNull.Value);
            command.Parameters.AddWithValue("@ReadTimeUTC", this.ReadTimeUTC.HasValue ? (object)this.ReadTimeUTC.Value : DBNull.Value);
            command.Parameters.AddWithValue("@DeleteTimeUTC", this.DeleteTimeUTC.HasValue ? (object)this.DeleteTimeUTC.Value : DBNull.Value);
        }

        public string SelectQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[SenderId]
                                      ,[ChainId]
                                      ,[ReceiverId]
                                      ,[CheckInId]
                                      ,[Title]
                                      ,[Content]
                                      ,[QueueTimeUTC]
                                      ,[PushTimeUTC]
                                      ,[ReadTimeUTC]
                                      ,[DeleteTimeUTC]
                                  FROM [dbo].[Message]
                                  WHERE [Id] = '{0}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id);
        }

        public string SelectAllQuery(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex=0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.SenderId = reader.GetGuid(colIndex++);
            this.ChainId = reader.GetGuid(colIndex++);
            this.ReceiverId = reader.GetGuid(colIndex++);
            
            if(!reader.IsDBNull(colIndex))
            {
                this.CheckInId = reader.GetGuid(colIndex);
            }
            colIndex++;

            this.Title = reader.GetString(colIndex++);
            this.Content = reader.GetString(colIndex++);
            this.QueueTimeUTC = reader.GetDateTime(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.PushTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.ReadTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.DeleteTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();
        }

        public static int CountPerChannel(SqlConnection connection, Guid groupId, Guid chainId, Guid receiverId, DateTime sinceTimeUtc)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = CountPerChannelQuery(groupId, chainId, receiverId, sinceTimeUtc);
                command.CommandTimeout = Database.TimeoutSecs;
                int count = (int)command.ExecuteScalar();
                return count;
            }
        }

        public static int CountPerCheckin(SqlConnection connection, Guid checkinId)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = CountPerCheckinQuery(checkinId);
                command.CommandTimeout = Database.TimeoutSecs;
                int count = (int)command.ExecuteScalar();
                return count;
            }
        }

        private static string CountPerChannelQuery(Guid groupId, Guid chainId, Guid receiverId, DateTime sinceTimeUtc)
        {
            string query = "SELECT COUNT (*) FROM [dbo].[Message] WHERE [GroupId] = '{0}' AND [ChainId] = '{1}' AND [ReceiverId] = '{2}' AND [QueueTimeUTC] >= '{3}'";
            return String.Format(CultureInfo.InvariantCulture, query, groupId, chainId, receiverId, sinceTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }

        private static string CountPerCheckinQuery(Guid checkinId)
        {
            string query = "SELECT COUNT (*) FROM [dbo].[Message] WHERE [CheckInId] = '{0}'";
            return String.Format(CultureInfo.InvariantCulture, query, checkinId);
        }

        private void Validate()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.SenderId == Guid.Empty)
            {
                throw new ArgumentException("Unknown SenderId");
            }

            if (this.ChainId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChainId");
            }

            if (this.ReceiverId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ReceiverId");
            }

            if (String.IsNullOrWhiteSpace(this.Title))
            {
                throw new ArgumentException("Invalid Title");
            }

            if (String.IsNullOrWhiteSpace(this.Content))
            {
                throw new ArgumentException("Invalid Content");
            }
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Choice : YummyZoneEntity, IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[Choice] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Choice]
                                ([GroupId]
                                ,[Id]
                                ,[Wording]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                        VALUES(
                                @GroupId,
                                @Id,
                                @Wording,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Choice]
                            SET [Wording] = @Wording,
                                [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                            WHERE [Id] = @Id AND [GroupId] = @GroupId;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            // check 'answer' table...
            string query = @"
                            declare @childCount int;
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[Answer] WHERE [AnswerChoiceId] = @Id);
                            IF(@childCount > 0)
                            BEGIN
                                UPDATE [dbo].[QuestionAndChoiceMap] SET [Status] = {0} WHERE [ChoiceId] = @Id AND [GroupId] = @GroupId;
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[Choice] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
                            END";

            query = Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, (int)Status.Removed);

            return query;
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@Id", this.Id);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Wording", this.Wording);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public string SelectQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"SELECT [GroupId],
                                    [Id],
                                    [Wording],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[Choice]                            
                            WHERE [Id] = '{0}' AND [GroupId] = '{1}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id, this.GroupId);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"SELECT [GroupId],
                                    [Id],
                                    [Wording],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[Choice]
                            WHERE [GroupId] = '{0}'
                            ORDER BY [CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Wording = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}

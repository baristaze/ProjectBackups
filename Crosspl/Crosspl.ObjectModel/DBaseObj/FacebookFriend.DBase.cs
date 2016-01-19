using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public partial class FacebookFriend : IEditable
    {
        public string InsertQueryInline()
        {
            string query = "INSERT INTO [dbo].[FacebookFriend] ([FacebookId1], [FacebookId2], [Friend2Name]) VALUES ({0}, {1}, N'{2}');";
            return String.Format(CultureInfo.InvariantCulture, query, this.FacebookId1, this.FacebookId2, this.Friend2Name.Replace("'", "''"));
        }

        public string MarkAsInvitedQuery()
        {
            string query = @"IF EXISTS(SELECT * FROM [dbo].[FacebookFriend] WHERE [FacebookId1] = {0} AND [FacebookId2] = {1})
                             BEGIN
	                            UPDATE [dbo].[FacebookFriend] SET [IsInvited] = 1, [InvitationTimeUTC] = GETUTCDATE() WHERE [FacebookId1] = {0} AND [FacebookId2] = {1} 
                             END
                             ELSE
                             BEGIN
	                            INSERT INTO [dbo].[FacebookFriend] ([FacebookId1], [FacebookId2], [IsInvited], [InvitationTimeUTC]) VALUES ({0}, {1}, 1, GETUTCDATE());	
                             END;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.FacebookId1, this.FacebookId2);
        }

        public string InsertOrUpdateQuery()
        {
            // insert only
            string query = @"
            declare @count int;
            set @count = (SELECT COUNT(*) FROM [dbo].[FacebookFriend] WHERE [FacebookId1] = @FacebookId1 AND [FacebookId2] = @FacebookId2);
            IF (@count = 0)
            BEGIN
                INSERT INTO [dbo].[FacebookFriend]
                       ([FacebookId1]
                       ,[FacebookId2]
                       ,[Friend2Name]
                       ,[IsInvited]
                       ,[InvitationTimeUTC]
                       ,[CreateTimeUTC])
                 VALUES
                       (@FacebookId1
                       ,@FacebookId2
                       ,@Friend2Name
                       ,@IsInvited
                       ,@InvitationTimeUTC
                       ,@CreateTimeUTC);
            END
            ELSE
            BEGIN
                UPDATE [dbo].[FacebookFriend]
                   SET [Friend2Name] = @Friend2Name
                      ,[IsInvited] = @IsInvited
                      ,[InvitationTimeUTC] = @InvitationTimeUTC
                 WHERE [FacebookId1] = @FacebookId1 AND [FacebookId2] = @FacebookId2;
            END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            return "DELETE FROM [dbo].[FacebookFriend] WHERE [FacebookId1] = @FacebookId1 AND [FacebookId2] = @FacebookId2;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.FacebookId1 <= 0)
            {
                throw new ArgumentException("Unknown FacebookId1");
            }

            if (this.FacebookId2 <= 0)
            {
                throw new ArgumentException("Unknown FacebookId2");
            }

            command.Parameters.AddWithValue("@FacebookId1", this.FacebookId1);
            command.Parameters.AddWithValue("@FacebookId2", this.FacebookId2);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                Database.AddSqlParamOrNull(command, "@Friend2Name", this.@Friend2Name);
                Database.AddSqlParamOrNull(command, "@IsInvited", this.@IsInvited);
                Database.AddSqlParamOrNull(command, "@InvitationTimeUTC", this.@InvitationTimeUTC);
                Database.AddSqlParamOrNull(command, "@CreateTimeUTC", this.@CreateTimeUTC);
            }
        }

        public string SelectQuery()
        {
            if (this.FacebookId1 <= 0)
            {
                throw new ArgumentException("Unknown FacebookId1");
            }

            if (this.FacebookId2 <= 0)
            {
                throw new ArgumentException("Unknown FacebookId2");
            }
            
            string query = @"   SELECT [FacebookId1]
                                      ,[FacebookId2]
                                      ,[Friend2Name]
                                      ,[IsInvited]
                                      ,[InvitationTimeUTC]
                                      ,[CreateTimeUTC]
                                  FROM [dbo].[FacebookFriend]
                                  WHERE [FacebookId1] = {0} AND [FacebookId2] = {1}";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.FacebookId1, this.FacebookId2);
        }

        public string SelectAllQuery(object filter)
        {
            long facebookId1 = 0;
            bool? invitedFilter = null;
            if (filter is long)
            {
                facebookId1 = (long)filter;
            }
            else // if (filter is FacebookFriend)
            {
                FacebookFriend ff = filter as FacebookFriend;
                if (ff != null)
                {
                    facebookId1 = ff.FacebookId1;
                    invitedFilter = ff.IsInvited;
                }
            }
                        
            if (facebookId1 <= 0)
            {
                throw new ArgumentException("Unknown FacebookId1");
            }

            string query = @"   SELECT [FacebookId1]
                                      ,[FacebookId2]
                                      ,[Friend2Name]
                                      ,[IsInvited]
                                      ,[InvitationTimeUTC]
                                      ,[CreateTimeUTC]
                                  FROM [dbo].[FacebookFriend]
                                  WHERE [FacebookId1] = {0}";

            if (invitedFilter.HasValue)
            {
                query += " AND [IsInvited] = " + (invitedFilter.Value ? "1" : "0");
            }

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, facebookId1);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.FacebookId1 = reader.GetInt64(colIndex++);
            this.FacebookId2 = reader.GetInt64(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Friend2Name = reader.GetString(colIndex);
            }
            colIndex++;

            this.IsInvited = reader.GetBoolean(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.InvitationTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Answer : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.QuestionId == Guid.Empty)
            {
                throw new ArgumentException("Unknown Question Id");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[Answer] WHERE [CheckInId] = @CheckInId AND [QuestionId] = @QuestionId);
                    IF (@count = 0)
                    BEGIN
                         INSERT INTO [dbo].[Answer](
				                    [CheckInId],
				                    [QuestionId],
				                    [AnswerYesNo],
	                                [AnswerRate],
	                                [AnswerChoiceId],
	                                [AnswerFreeText],
				                    [CreateTimeUTC],
				                    [LastUpdateTimeUTC])
                         VALUES(
				                    @CheckInId,
				                    @QuestionId,
				                    @AnswerYesNo,
	                                @AnswerRate,
	                                @AnswerChoiceId,
	                                @AnswerFreeText,
				                    @CreateTimeUTC,
				                    @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Answer]
                           SET [AnswerYesNo] = @AnswerYesNo,
	                           [AnswerRate] = @AnswerRate,
	                           [AnswerChoiceId] = @AnswerChoiceId,
	                           [AnswerFreeText] = @AnswerFreeText,
                               [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                         WHERE [CheckInId] = @CheckInId AND [QuestionId] = @QuestionId;
                    END";

            return Database.ShortenQuery(query);
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.QuestionId == Guid.Empty)
            {
                throw new ArgumentException("Unknown Question Id");
            }

            command.Parameters.AddWithValue("@CheckInId", this.CheckInId);
            command.Parameters.AddWithValue("@QuestionId", this.QuestionId);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@AnswerYesNo", this.AnswerYesNo.HasValue ? this.AnswerYesNo.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@AnswerRate", this.AnswerRate.HasValue ? this.AnswerRate.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@AnswerChoiceId", this.AnswerChoiceId.HasValue ? this.AnswerChoiceId.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@AnswerFreeText", String.IsNullOrWhiteSpace(this.AnswerFreeText) ? (object)DBNull.Value : this.AnswerFreeText);

                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public string DeleteQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.QuestionId == Guid.Empty)
            {
                throw new ArgumentException("Unknown Question Id");
            }

            return @"DELETE FROM [dbo].[Answer] WHERE [CheckInId] = @CheckInId AND [QuestionId] = @QuestionId;";
        }

        public string SelectQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.QuestionId == Guid.Empty)
            {
                throw new ArgumentException("Unknown Question Id");
            }

            string query = @"SELECT [A].[CheckInId],
                                    [A].[QuestionId],
                                    [Q].[Wording],
                                    [A].[AnswerYesNo],
                                    [A].[AnswerRate],
                                    [A].[AnswerChoiceId],
                                    [C].[Wording],
                                    [A].[AnswerFreeText],
                                    [A].[CreateTimeUTC],
                                    [A].[LastUpdateTimeUTC]
                              FROM [dbo].[Answer] [A]
                              JOIN [dbo].[Question] [Q] ON [A].[QuestionId] = [Q].[Id]
                              LEFT JOIN [dbo].[Choice] [C] ON [A].[AnswerChoiceId] = [C].[Id]
                              WHERE [CheckInId] = '{0}' AND [QuestionId] = '{1}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.CheckInId, this.QuestionId);
        }

        public string SelectAllQuery(Guid checkInId)
        {
            if (checkInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            string answerFilter = NonEmptyAnswerFilter("[A].");

            string query = @"SELECT [A].[CheckInId],
                                    [A].[QuestionId],
                                    [Q].[Wording],
                                    [A].[AnswerYesNo],
                                    [A].[AnswerRate],
                                    [A].[AnswerChoiceId],
                                    [C].[Wording],
                                    [A].[AnswerFreeText],
                                    [A].[CreateTimeUTC],
                                    [A].[LastUpdateTimeUTC]
                              FROM [dbo].[Answer] [A]
                              JOIN [dbo].[Question] [Q] ON [A].[QuestionId] = [Q].[Id]
                              LEFT JOIN [dbo].[Choice] [C] ON [A].[AnswerChoiceId] = [C].[Id]
                              WHERE [CheckInId] = '{0}' AND ({1});";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, checkInId, answerFilter);
        }

        public static string SelectMultipleQuery(CheckinList checkinList)
        {
            if (checkinList == null || checkinList.Count == 0)
            {
                throw new ArgumentException("CheckinList parameter is null or empty");
            }

            string checkinIdfilter = Checkin.BulkFilter(checkinList, "[A].[CheckInId]");
            string answerFilter = NonEmptyAnswerFilter("[A].");

            string query = @"SELECT [A].[CheckInId],
                                    [A].[QuestionId],
                                    [Q].[Wording],
                                    [A].[AnswerYesNo],
                                    [A].[AnswerRate],
                                    [A].[AnswerChoiceId],
                                    [C].[Wording],
                                    [A].[AnswerFreeText],
                                    [A].[CreateTimeUTC],
                                    [A].[LastUpdateTimeUTC]
                              FROM [dbo].[Answer] [A]
                              JOIN [dbo].[Question] [Q] ON [A].[QuestionId] = [Q].[Id]
                              LEFT JOIN [dbo].[Choice] [C] ON [A].[AnswerChoiceId] = [C].[Id]
                              WHERE ({0}) AND ({1});";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, answerFilter, checkinIdfilter);
        }

        private static string NonEmptyAnswerFilter(string tableAlias)
        {
            string filter = "{0}[AnswerYesNo] is NOT NULL OR " +
                            "{0}[AnswerRate] is NOT NULL OR " +
                            "{0}[AnswerChoiceId] is NOT NULL OR " +
                            "{0}[AnswerFreeText] is NOT NULL";

            return String.Format(CultureInfo.InvariantCulture, filter, tableAlias);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.CheckInId = reader.GetGuid(colIndex++);
            this.QuestionId = reader.GetGuid(colIndex++);
            this.QuestionText = reader.GetString(colIndex); // not ++ yet

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.AnswerYesNo = reader.GetBoolean(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.AnswerRate = reader.GetByte(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.AnswerChoiceId = reader.GetGuid(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.AnswerChoiceText = reader.GetString(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.AnswerFreeText = reader.GetString(colIndex);
            }

            colIndex++;
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}

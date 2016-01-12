using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class Question : YummyZoneEntity, IEditable
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
                    set @count = (SELECT COUNT(*) FROM [dbo].[Question] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Question]
                                ([GroupId]
                                ,[Id]
                                ,[Type]
                                ,[Wording]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                        VALUES(
                                @GroupId,
                                @Id,
                                @Type,
                                @Wording,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Question]
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
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[Answer] WHERE [QuestionId] = @Id);
                            IF (@childCount > 0)
                            BEGIN
                                UPDATE [dbo].[ChainAndQuestionMap] SET [Status] = {0} WHERE [QuestionId] = @Id AND [GroupId] = @GroupId;
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[Choice] WHERE [Id] IN (
	                                SELECT [C].[Id] 
		                                FROM [dbo].[Choice] [C]
		                                JOIN [dbo].[QuestionAndChoiceMap] [M] ON [C].[Id] = [M].[ChoiceId]
		                                WHERE [C].[GroupId] = @GroupId AND [M].[QuestionId] = @Id);

                                DELETE FROM [dbo].[Question] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
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
                command.Parameters.AddWithValue("@Type", (byte)this.QuestionType);
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
		                            [Type],
                                    [Wording],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[Question]                            
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
		                            [Type],
                                    [Wording],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[Question]
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
            this.QuestionType = (QuestionType)reader.GetByte(colIndex++);
            this.Wording = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public static List<QuestionList> SelectSurveyQuestionsOfChain(SqlConnection connection, Guid groupId, Guid chainId)
        {
            List<Guid> filteredQuestionIds = new List<Guid>();
            MapListChainToQuestion maps = Database.SelectAll<MapChainToQuestion, MapListChainToQuestion>(
                connection, null, groupId, Database.TimeoutSecs);

            foreach (MapChainToQuestion map in maps)
            {
                if (map.ChainId == chainId && map.Status != Status.Removed)
                {
                    filteredQuestionIds.Add(map.QuestionId);
                }
            }

            QuestionList filteredQuestions = new QuestionList();
            if (filteredQuestionIds.Count > 0)
            {
                QuestionList questions = Database.SelectAll<Question, QuestionList>(connection, null, groupId, Database.TimeoutSecs);

                // do not change the order of for loops
                foreach (Guid questionId in filteredQuestionIds)
                {
                    Question q = questions[questionId];
                    if (q != null)
                    {
                        filteredQuestions.Add(q);
                    }
                }
            }

            QuestionList raters = new QuestionList();
            QuestionList yesNo = new QuestionList();
            QuestionList multiple = new QuestionList();
            QuestionList openEnded = new QuestionList();
            foreach (Question q in filteredQuestions)
            {
                if (q.QuestionType == QuestionType.Rate)
                {
                    raters.Add(q);
                }
                else if (q.QuestionType == QuestionType.YesNo)
                {
                    yesNo.Add(q);
                }
                else if (q.QuestionType == QuestionType.MultiChoice)
                {
                    multiple.Add(q);
                }
                else if (q.QuestionType == QuestionType.FreeText)
                {
                    openEnded.Add(q);
                }
            }

            if (multiple.Count > 0)
            {
                ChoiceList choices = Database.SelectAll<Choice, ChoiceList>(connection, null, groupId, Database.TimeoutSecs);
                MapListQuestionToChoice choiceMaps = Database.SelectAll<MapQuestionToChoice, MapListQuestionToChoice>(
                    connection, null, groupId, Database.TimeoutSecs);

                foreach (MapQuestionToChoice choiceMap in choiceMaps)
                {
                    if (choiceMap.Status != Status.Removed)
                    {
                        Question q = multiple[choiceMap.QuestionId];
                        Choice c = choices[choiceMap.ChoiceId];

                        if (q != null && c != null)
                        {
                            q.Choices.Add(c);
                        }
                    }
                }
            }

            List<QuestionList> allQuestions = new List<QuestionList>();
            allQuestions.Add(raters);
            allQuestions.Add(yesNo);
            allQuestions.Add(multiple);
            allQuestions.Add(openEnded);

            return allQuestions;
        }
    }
}

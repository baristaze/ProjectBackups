using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuAddUpdate httphandler to receive files and save them to the server.
    /// </summary>
    public class QuestionAddUpdate : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            int questionType = this.GetMandatoryInt(context, "qtype", "Question Type", 1, 4, Source.Url);

            Question question = new Question();
            question.GroupId = identity.GroupId;
            question.QuestionType = (QuestionType)questionType;

            List<IEditable> entities = new List<IEditable>();
            entities.Add(question);

            // get the question id
            bool isNew = true;
            string questionId = context.Request.Params["qid"];
            if (!String.IsNullOrWhiteSpace(questionId))
            {
                isNew = false;

                // if we have a valid ID, then this is an update case
                question.Id = this.GetMandatoryGuid(context, "qid", "Question Id", Source.Url);

                // get the question from database
                if (!Database.Select(question, Helpers.ConnectionString, Database.TimeoutSecs))
                {
                    string msg = "There is not a question with the given Id: " + question.Id.ToString();
                    throw new YummyZoneArgumentException(msg, "mid");
                }

                // update the time
                question.LastUpdateTimeUTC = DateTime.UtcNow;
            }
            else
            {
                // add new case... we need to add the map as well
                MapChainToQuestion map = new MapChainToQuestion();
                map.GroupId = identity.GroupId;
                map.ChainId = identity.ChainId;
                map.QuestionId = question.Id;

                entities.Add(map);
            }
            
            // get text
            question.Wording = this.GetMandatoryString(context, "qtext", "Question", question.GetMaxWordingLength(), Source.Url);
            
            // get the choices
            bool simpleAddInsert = true;
            List<Guid> resultChoiceIds = new List<Guid>();
            if (questionType == 3)
            {
                int choiceLen = 5;
                Guid[] choiceIds = new Guid[choiceLen];
                string[] choiceWords = new string[choiceLen];
                for (int x = 0; x < choiceLen; x++)
                {
                    string xStr = x.ToString();
                    choiceIds[x] = this.GetGuid(context, "chid" + xStr, "Choice Id " + xStr, Source.Url);
                    choiceWords[x] = this.GetString(context, "chwrd" + xStr, "Choice Text " + xStr, Choice.MaxWording, Source.Url);
                }

                if (isNew)
                {
                    for (int x = 0; x < choiceLen; x++)
                    {
                        if (!String.IsNullOrWhiteSpace(choiceWords[x]))
                        {
                            Choice choice = new Choice(identity.GroupId, choiceWords[x].Trim());
                            MapQuestionToChoice map = new MapQuestionToChoice(identity.GroupId, question.Id, choice.Id, (byte)x);
                            entities.Add(choice);
                            entities.Add(map);
                            resultChoiceIds.Add(choice.Id);
                        }
                    }
                }
                else
                {
                    simpleAddInsert = false;

                    resultChoiceIds = UpdateAll(entities, identity, question.Id, choiceIds, choiceWords);
                }
            }

            if (simpleAddInsert)
            {
                // no need to catch... 
                Database.InsertOrUpdate(entities, Helpers.ConnectionString);
            }

            string returnVal = question.Id.ToString("N");
            if (questionType == 3)
            {
                foreach (Guid choiceId in resultChoiceIds)
                {
                    returnVal += ";" + choiceId.ToString("N");
                }
            }

            return returnVal;
        }

        private List<Guid> UpdateAll(
            List<IEditable> entities,
            TenantIdentity identity,
            Guid questionId,
            Guid[] choiceIds,
            string[] choiceWords)
        {
            bool allSucceeded = false;
            Exception exception = null;
            List<Guid> resultChoiceIds = new List<Guid>();
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        // handle question and its map to chain
                        foreach (IEditable entity in entities)
                        {
                            Database.InsertOrUpdate(entity, connection, trans, Database.TimeoutSecs);
                        }

                        // handle choices and their maps to question
                        ChoiceOperationList newChoiceOperationList = GetChoiceOperations(
                            connection, trans, identity, questionId, choiceIds, choiceWords);

                        // update the choices
                        byte order = 0;
                        MapListQuestionToChoice newMaps = new MapListQuestionToChoice();
                        foreach (ChoiceOperation chop in newChoiceOperationList)
                        {
                            if (chop.Action == ChoiceOperation.Operation.Delete)
                            {
                                Database.Delete(chop.Choice, connection, trans, Database.TimeoutSecs);
                            }
                            else
                            {
                                Database.InsertOrUpdate(chop.Choice, connection, trans, Database.TimeoutSecs);

                                // prepare the map
                                resultChoiceIds.Add(chop.Choice.Id);
                                MapQuestionToChoice map = new MapQuestionToChoice(identity.GroupId, questionId, chop.Choice.Id, order++);
                                if (chop.Action == ChoiceOperation.Operation.Add)
                                {
                                    Database.InsertOrUpdate(map, connection, trans, Database.TimeoutSecs);
                                }
                                else
                                {
                                    // reorder will handle this
                                    newMaps.Add(map);
                                }
                            }
                        }

                        // update choice maps
                        if (newMaps.Count > 0)
                        {
                            Database.Reorder(newMaps, false, connection, trans, Database.TimeoutSecs);
                        }

                        // commit
                        trans.Commit();
                        allSucceeded = true;
                    }
                    catch (YummyZoneArgumentException exc)
                    {
                        exception = exc;
                    }
                    catch (YummyZoneException exc)
                    {
                        exception = exc;
                    }
                    finally
                    {
                        if (!allSucceeded)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            if (!allSucceeded)
            {
                if (exception != null)
                {
                    throw exception;
                }
                else
                {
                    throw new YummyZoneException("Couldn't commit to the database");
                }
            }

            return resultChoiceIds;
        }

        private ChoiceOperationList GetChoiceOperations(
            SqlConnection connection, 
            SqlTransaction transaction, 
            TenantIdentity identity, 
            Guid questionId, 
            Guid[] choiceIds, 
            string[] choiceWords)
        {   
            MapListQuestionToChoice existingChoiceMaps = Database.SelectAll<MapQuestionToChoice, MapListQuestionToChoice>(
                connection, transaction, identity.GroupId, Database.TimeoutSecs);
            MapListQuestionToChoice choiceMapsForThisQuestion = existingChoiceMaps.Filter(questionId, Status.Active);
                        
            ChoiceList existingChoices = Database.SelectAll<Choice, ChoiceList>(
                connection, transaction, identity.GroupId, Database.TimeoutSecs);
            ChoiceList existingChoicesForThisQuestion = existingChoices.Filter(choiceMapsForThisQuestion);

            ChoiceOperationList newChoiceOperationList = CreateNewChoiceOperations(identity, choiceIds, choiceWords);

            //ChoiceOperationList extraActionsBasedOnComparisons = new ChoiceOperationList();
            foreach (Choice existingChoice in existingChoicesForThisQuestion)
            {
                ChoiceOperation cop = newChoiceOperationList[existingChoice.Id];
                if (cop == null)
                {
                    // there was a choice previously. however it didn't come out in this new query
                    // we will treat this as a 'delete choice' operation.
                    /*
                    cop = new ChoiceOperation();
                    cop.Choice = existingChoice;
                    cop.Action = ChoiceOperation.Operation.Delete;
                    extraActionsBasedOnComparisons.Add(cop);
                     */
                    throw new YummyZoneException("Invalid request! Please refresh the page and try again");
                }
            }

            // verify updates and deletes
            foreach (ChoiceOperation chop in newChoiceOperationList)
            {
                // update or delete case
                if (chop.Action != ChoiceOperation.Operation.Add)
                { 
                    // this choice must exist in current picture already
                    Choice matchingChoice = existingChoicesForThisQuestion[chop.Choice.Id];
                    if (matchingChoice == null)
                    {
                        throw new YummyZoneException("Invalid query! Please refresh the page and try again");
                    }
                }
            }

            //newChoiceOperationList.AddRange(extraActionsBasedOnComparisons);

            return newChoiceOperationList;
        }

        private ChoiceOperationList CreateNewChoiceOperations(TenantIdentity identity, Guid[] choiceIds, string[] choiceWords)
        {
            ChoiceOperationList copList = new ChoiceOperationList();

            for (int x = 0; x < choiceIds.Length; x++)
            {
                Choice choice = new Choice();                
                choice.GroupId = identity.GroupId;
                
                if (choiceIds[x] != Guid.Empty)
                {
                    choice.Id = choiceIds[x];
                }

                if (!String.IsNullOrWhiteSpace(choiceWords[x]))
                {
                    choice.Wording = choiceWords[x].Trim();
                }

                ChoiceOperation co = new ChoiceOperation();
                co.Choice = choice;

                bool omit = false;
                if (choiceIds[x] == Guid.Empty && String.IsNullOrWhiteSpace(choiceWords[x]))
                {
                    // omit
                    omit = true;
                }
                else if (choiceIds[x] == Guid.Empty && !String.IsNullOrWhiteSpace(choiceWords[x]))
                {
                    // add
                    co.Action = ChoiceOperation.Operation.Add;
                }
                else if (choiceIds[x] != Guid.Empty && String.IsNullOrWhiteSpace(choiceWords[x]))
                {
                    // delete                    
                    co.Action = ChoiceOperation.Operation.Delete;
                }
                else if (choiceIds[x] != Guid.Empty && !String.IsNullOrWhiteSpace(choiceWords[x]))
                {
                    // update
                    co.Action = ChoiceOperation.Operation.Update;
                }

                if (!omit)
                {
                    copList.Add(co);
                }
            }

            return copList;
        }

        private class ChoiceOperation
        {
            public enum Operation { Add, Delete, Update }

            public Choice Choice { get; set; }
            public Operation Action { get; set; }
        }

        private class ChoiceOperationList : List<ChoiceOperation> 
        {
            public ChoiceOperation this[Guid id]
            {
                get
                {
                    foreach (ChoiceOperation item in this)
                    {
                        if (item.Choice.Id == id)
                        {
                            return item;
                        }
                    }

                    return null;
                }
            }
        }
    }
}
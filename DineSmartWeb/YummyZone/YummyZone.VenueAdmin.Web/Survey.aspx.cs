using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Survey : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = this.UpdateIdentitySection();
            List<QuestionList> allQuestions = new List<QuestionList>();

            if (identity != null)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();

                    allQuestions = Question.SelectSurveyQuestionsOfChain(
                        connection, identity.GroupId, identity.ChainId);
                }
            }
            else
            {
                allQuestions.Add(new QuestionList());
                allQuestions.Add(new QuestionList());
                allQuestions.Add(new QuestionList());
                allQuestions.Add(new QuestionList());
            }

            rateQuestionsRepeater.DataSource = allQuestions[0];
            rateQuestionsRepeater.DataBind();

            yesNoQuestionsRepeater.DataSource = allQuestions[1];
            yesNoQuestionsRepeater.DataBind();

            multiChoiceRepeater.DataSource = allQuestions[2];
            multiChoiceRepeater.DataBind();

            openEndedQuestionsRepeater.DataSource = allQuestions[3];
            openEndedQuestionsRepeater.DataBind();
        }
    }
}
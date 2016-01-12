using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// QuestionDeleteDisable httphandler
    /// </summary>
    public class QuestionDeleteDisable: YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            Question question = new Question();
            question.Id = this.GetMandatoryGuid(context, "qid", "Question Id", Source.Url);
            question.GroupId = identity.GroupId;
            question.LastUpdateTimeUTC = DateTime.UtcNow;
            
            List<IEditable> entities = new List<IEditable>();
            entities.Add(question);

            bool isDelete = this.GetDeleteOrDisableAction(context, Source.Url);
            if (isDelete)
            {
                Database.Delete(entities, Helpers.ConnectionString);
            }
            else
            {
                throw new NotImplementedException();
            }

            return 1; // success
        }
    }
}
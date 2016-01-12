using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// ChoicesReorder httphandler
    /// </summary>
    public class ChoicesReorder : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            Guid questionId = GetMandatoryGuid(context, "qid", "question id", Source.Url);
            List<Guid> choiceIds = GetMandatoryGuidList(context, "chids", "choice ids");

            byte orderIndex = 0;
            MapListQuestionToChoice orderedEntities = new MapListQuestionToChoice();
            foreach (Guid choiceId in choiceIds)
            {
                MapQuestionToChoice map = new MapQuestionToChoice();
                map.GroupId = identity.GroupId;
                map.QuestionId = questionId;
                map.ChoiceId = choiceId;
                map.OrderIndex = orderIndex++;

                orderedEntities.Add(map);
            }

            Database.Reorder(orderedEntities, false, Helpers.ConnectionString);

            return 1; // success
        }
    }
}
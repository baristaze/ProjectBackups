using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// QuestionsReorder httphandler
    /// </summary>
    public class QuestionsReorder : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            List<Guid> questionIds = GetMandatoryGuidList(context, "qids", "question ids");

            byte orderIndex = 0;
            MapListChainToQuestion orderedEntities = new MapListChainToQuestion();
            foreach (Guid questionId in questionIds)
            {
                MapChainToQuestion map = new MapChainToQuestion();
                map.GroupId = identity.GroupId;
                map.ChainId = identity.ChainId;
                map.QuestionId = questionId;
                map.OrderIndex = orderIndex++;

                orderedEntities.Add(map);
            }

            Database.Reorder(orderedEntities, false, Helpers.ConnectionString);

            return 1; // success
        }
    }
}
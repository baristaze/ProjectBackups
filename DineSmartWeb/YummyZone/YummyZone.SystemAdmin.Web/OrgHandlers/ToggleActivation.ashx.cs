using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;
using System.Globalization;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    /// <summary>
    /// AddVenue httphandler
    /// </summary>
    public class ToggleActivation : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            SystemUser user = LoginHelper.GetIdentityFromAuth(context.Request, true);

            ObjectType objType = (ObjectType)this.GetMandatoryInt(context, "type", "Business Type", (int)ObjectType.Venue, (int)ObjectType.Group, Source.Url);
            if (objType != ObjectType.Venue)
            {
                throw new YummyZoneException("Not Supported at this time");
            }

            Guid objectId = this.GetMandatoryGuid(context, "id", "Business Id", Source.Url);
            int action = this.GetMandatoryInt(context, "act", "Action", 0, 1, Source.Url);
            
            string table = string.Empty;
            if(objType == ObjectType.Venue)
            {
                table = "[Venue]";
            }
            else if(objType == ObjectType.Chain)
            {
                table = "[Chain]";
            }
            else if(objType == ObjectType.Group)
            {
                table = "[Group]";
            }
            else
            {
                throw new YummyZoneException("Unsupported business type");
            }

            VenueStatus nextStat = VenueStatus.Active;
            if(action == 0)
            {
                nextStat = VenueStatus.Draft;
            }

            string query = "UPDATE [dbo].{0} SET [Status] = {1} WHERE [Id] = '{2}';";
            query = String.Format(
                CultureInfo.InvariantCulture, 
                query, 
                table,
                (int)nextStat,
                objectId);

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandTimeout = Database.TimeoutSecs;
                    command.ExecuteNonQuery();
                }
            }

            return 0;
        }
    }
}
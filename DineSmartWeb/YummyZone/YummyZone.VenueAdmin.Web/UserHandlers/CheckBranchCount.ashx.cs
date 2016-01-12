using System;
using System.Data.SqlClient;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class CheckBranchCount : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            if (identity.UserType == UserType.Customer)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();
                    return UserRole.ReadBranchCountForUser(connection, identity.GroupId, identity.UserId);
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
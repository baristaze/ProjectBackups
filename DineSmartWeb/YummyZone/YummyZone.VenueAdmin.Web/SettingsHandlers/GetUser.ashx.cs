using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// GetUser httphandler
    /// </summary>
    public class GetUser : YummyZoneHttpHandlerJson<GetUserResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            string email = this.GetMandatoryString(context, "email", "Email Address", 300, Source.Url);
            if (!StringHelpers.IsValidEmail(email))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                
                List<MemberUser> members = MemberUser.Select(
                    connection, null, identity.GroupId, identity.VenueId, Guid.Empty);

                MemberUser theUser = null;
                foreach (MemberUser user in members)
                {
                    if (String.Compare(user.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        theUser = user;
                        break;
                    }

                }

                GetUserResponse response = new GetUserResponse();
                if (theUser != null)
                {
                    response.User = theUser;
                }
                else
                {
                    throw new YummyZoneArgumentException("User info couldn't be retrieved. Please refresh the page");
                }
                
                return response;
            }
        }
    }

    [DataContract]
    public class GetUserResponse : BaseJsonResponse
    {
        [DataMember]
        public MemberUser User { get; set; }
    }
}
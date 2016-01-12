using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class Authenticate : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            string userEmail = this.GetMandatoryString(context, "userEmail", "Email Address", 200, Source.Form, false);
            if (!StringHelpers.IsValidEmail(userEmail))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }
            
            string userPassword = this.GetMandatoryString(context, "userPassword", "Password", 100, Source.Form, false);
                        
            string identity = LoginHelper.GetIdentityKey(userEmail, userPassword);

            if (String.IsNullOrWhiteSpace(identity))
            {
                throw new YummyZoneException("Authentication failed");
            }
            
            return Guid.NewGuid().ToString("N");
        }
    }
}
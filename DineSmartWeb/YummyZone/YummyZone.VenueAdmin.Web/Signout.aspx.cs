using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Globalization;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Signout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(this.Request, false);
            if (identity != null && identity.UserType != UserType.Customer)
            {
                string root = Helpers.FullyQualifiedDomainUrl(this.Context);

                string[] nameTokens = identity.UserFriendlyName.Split(' ');
                if (nameTokens.Length > 0)
                {
                    string first = String.Join(" ", nameTokens, 0, nameTokens.Length - 1);
                    string last = nameTokens[nameTokens.Length - 1];

                    string supportAgentIdentity = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0}[;]{1}[;]{2}[;]{3}[;]{4}",
                        identity.UserId,
                        identity.UserType == UserType.SystemAdmin ? "1" : "0",
                        identity.UserEmail,
                        first,
                        last);

                    FormsAuthentication.SetAuthCookie(supportAgentIdentity, false);
                    this.Response.Redirect(root, false);
                    return;
                }
            }

            FormsAuthentication.SignOut();
            this.Response.Redirect("Default.aspx");
        }
    }
}
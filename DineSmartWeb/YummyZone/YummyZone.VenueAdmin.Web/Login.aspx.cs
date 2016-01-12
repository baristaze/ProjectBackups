using System;
using System.Web.Security;
using System.Data.SqlClient;
using System.Globalization;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Login : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string returnUrl = this.Request.QueryString["ReturnUrl"];
                if (returnUrl != null && (returnUrl == "/" || returnUrl == "%2f"))
                {
                    this.Response.Redirect("Default.aspx");
                }
                else
                {
                    string encodedEmail = this.Request.Params["m"];
                    string encodedError = this.Request.Params["r"];

                    if (!String.IsNullOrWhiteSpace(encodedEmail))
                    {
                        string error = String.Empty;
                        string email = AsciiBase64Codex.Decode(encodedEmail);

                        if (!String.IsNullOrWhiteSpace(encodedError))
                        {
                            error = AsciiBase64Codex.Decode(encodedError);
                        }

                        this.emailHint.InnerText = email;
                        this.errorHint.InnerText = error;
                    }
                    else
                    {
                        string branchId = this.Request.QueryString["branchId"];
                        if (!String.IsNullOrWhiteSpace(branchId))
                        {
                            string identity = this.ChangeBranch(branchId);
                            if (!String.IsNullOrWhiteSpace(identity))
                            {
                                FormsAuthentication.SetAuthCookie(identity, false);
                                this.Response.Redirect(returnUrl.Trim(), false);
                            }
                        }
                    }
                }
            }
        }
        
        protected void HiddenButtonClicked(object sender, EventArgs e)
        {
            this.PerformLogin();
        }
        
        protected void PerformLogin()
        {
            string email = this.userEmail2.Value.Trim();
            string pswd = this.userPassword2.Value;

            LoginHelper.CheckLoginInput(email, pswd, true);
            
            string identity = LoginHelper.GetIdentityKey(email, pswd);

            if (String.IsNullOrWhiteSpace(identity))
            {
                throw new YummyZoneException("Authentication failed");
            }

            // FormsAuthentication.RedirectFromLoginPage(identity, false);

            string returnUrl = this.GetReturnUrl(true);
            if (String.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "Feedbacks.aspx";
            }

            FormsAuthentication.SetAuthCookie(identity, false);
            this.Response.Redirect(returnUrl.Trim(), false);
        }

        protected string GetReturnUrl(bool trim2F)
        {
            string returnUrl = null;
            if (this.Request.UrlReferrer != null && this.Request.UrlReferrer.Query != null)
            {
                string returnUrlKey = "ReturnUrl=";
                string referrer = this.Request.UrlReferrer.Query;
                int index = referrer.IndexOf(returnUrlKey, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    index += returnUrlKey.Length;
                    returnUrl = referrer.Substring(index);
                    returnUrl = returnUrl.TrimStart(' ', '/');
                    if (returnUrl.StartsWith("%2f") && trim2F)
                    {
                        returnUrl = returnUrl.Substring(3);
                    }
                }
            }

            return returnUrl;
        }

        protected string ChangeBranch(string branchId)
        {
            Guid targetVenueId = Guid.Empty;
            if (!Guid.TryParse(branchId, out targetVenueId))
            {
                return null;
            }

            if (targetVenueId == Guid.Empty)
            {
                return null;
            }
            
            // change branch request
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(this.Request, false);
            if (identity == null)
            {
                return null;
            }

            if (identity.UserType != UserType.Customer)
            {
                return null;
            }

            Venue venue = null;
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                string query = Venue.SelectAllByUserId(identity.GroupId, identity.UserId);
                VenueList venueList = Database.SelectAll<Venue, VenueList>(connection, null, query, Database.TimeoutSecs);
                venue = venueList[targetVenueId];
            }

            if (venue == null)
            {
                return null;
            }

            return String.Format(
                        CultureInfo.InvariantCulture,
                        "0[;]{0}[;]{1}[;]{2}[;]{3}[;]{4}[;]{5}[;]{6}",
                        venue.GroupId,              // 0
                        venue.ChainId,              // 1
                        venue.Id,                   // 2
                        identity.UserId,            // 3
                        identity.UserEmail,         // 4
                        identity.UserFriendlyName,  // 5                        
                        venue.Name);                // 6
        }
    }
}
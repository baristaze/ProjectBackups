using System;
using System.Text;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public class YummyZonePage : System.Web.UI.Page
    {
        protected TenantIdentity UpdateIdentitySection()
        {
            HtmlGenericControl ctrl;
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(this.Request, false);
            if (identity != null)
            {
                ctrl = this.Page.FindControl("brnchName") as HtmlGenericControl;
                if (ctrl != null)
                {
                    ctrl.InnerHtml = identity.VenueName;
                }

                ctrl = this.Page.FindControl("usrName") as HtmlGenericControl;
                if (ctrl != null)
                {
                    ctrl.InnerHtml = identity.UserFriendlyName;
                }

                ctrl = this.Page.FindControl("loginWrp") as HtmlGenericControl;
                if (ctrl != null)
                {
                    ctrl.Visible = false;
                }
            }
            else
            {
                ctrl = this.Page.FindControl("identityWrp") as HtmlGenericControl;
                if (ctrl != null)
                {
                    ctrl.Visible = false;
                }
            }

            return identity;
        }

        protected void UpdateDashboardMenuLink(TenantIdentity identity)
        {
            Control ctrl = this.FindControl("menuLink2");
            if (ctrl != null)
            {
                ctrl.Visible = (identity != null);
            }

            ctrl = this.FindControl("menuSep2");
            if (ctrl != null)
            {
                ctrl.Visible = (identity != null);
            }
        }

        protected void MainMenuPublicPageCheckLoginRedirection()
        {
            if (this.Request.Params["login"] != null && this.Request.Params["login"] == "1")
            {
                string email = this.Request.Form["bizEmail"];
                string pswd = this.Request.Form["password"];

                if (email != null && pswd != null)
                {
                    if (email == "your email")
                    {
                        email = string.Empty;
                    }

                    this.SafeLogin(email, pswd);
                }
            }
        }

        protected void SafeLogin(string email, string pswd)
        {
            Exception ex = null;

            try
            {
                this.Login(email, pswd);
            }
            catch (YummyZoneArgumentException e)
            {
                ex = e;
            }
            catch (YummyZoneException e)
            {
                ex = e;
            }
            catch (Exception e)
            {
                string s = e.Message;
                ex = new YummyZoneException("Unknown error", e);
            }
            finally
            {
                if (ex != null)
                {
                    string encodedEmail = AsciiBase64Codex.Encode(email.Trim());
                    string encodedError = AsciiBase64Codex.Encode(ex.Message);
                    string target = String.Format(
                        CultureInfo.InvariantCulture,
                        "Login.aspx?m={0}&r={1}",
                        encodedEmail,
                        encodedError);

                    this.Response.Redirect(target);
                }
            }
        }

        protected void Login(string email, string pswd)
        {
            email = email.Trim();
            LoginHelper.CheckLoginInput(email, pswd, true);

            string identity = LoginHelper.GetIdentityKey(email, pswd);

            if (String.IsNullOrWhiteSpace(identity))
            {
                throw new YummyZoneException("Authentication failed");
            }

            //FormsAuthentication.RedirectFromLoginPage(identity, false);

            FormsAuthentication.SetAuthCookie(identity, false);
            this.Response.Redirect("Feedbacks.aspx", false);
        }

        protected TenantIdentity SinglePublicPageInit()
        {   
            TenantIdentity identity = this.UpdateIdentitySection();
            Control ctrl = this.FindControl("topMenuLoginStrip");
            if (ctrl != null)
            {
                ctrl.Visible = (identity != null);
            }
            return identity;
        }
    }
}
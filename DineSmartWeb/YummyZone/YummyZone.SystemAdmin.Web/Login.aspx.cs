using System;
using System.Web.Security;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    public partial class Login : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                throw new YummyZoneException("No matching email and password");
            }

            FormsAuthentication.RedirectFromLoginPage(identity, false);
        }
    }
}
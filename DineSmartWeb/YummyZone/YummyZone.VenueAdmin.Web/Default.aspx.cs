using System;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Default : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                TenantIdentity identity = this.UpdateIdentitySection();

                this.UpdateDashboardMenuLink(identity);
                
                this.joinNowBtnDiv.Visible = (identity == null);
                this.ctrlPanelLinkWrp.Visible = (identity != null);
            }
            else 
            {
                this.MainMenuPublicPageCheckLoginRedirection(); 
            }
        }
    }
}
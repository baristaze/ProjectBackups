using System;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Settings : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = this.UpdateIdentitySection();
            if (identity != null)
            {
                this.currentUserName.InnerText = identity.UserEmail;

                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = (new LogoImage()).ExistQuery(identity.ChainId);
                        command.CommandTimeout = Database.TimeoutSecs;
                        int count = (int)command.ExecuteScalar();
                        if (count == 1)
                        {
                            customerLogoImg.Src = "SettingsHandlers/LogoDownload.ashx?fid=" + identity.ChainId.ToString();
                        }
                        else
                        {
                            customerLogoImg.Src = "Images/venueDefaultImage.png";
                        }
                    }

                    List<MemberUser> members = MemberUser.Select(connection, null, identity.GroupId, identity.VenueId, Guid.Empty);

                    if (identity.UserType == UserType.Customer)
                    {
                        MemberUser mySelf = null;
                        for (int x = 0; x < members.Count; x++)
                        {
                            if (members[x].UserId == identity.UserId)
                            {
                                mySelf = members[x];
                                members.RemoveAt(x);
                                mySelf.IsReadOnly = true;
                                break;
                            }
                        }

                        if (mySelf != null)
                        {
                            members.Insert(0, mySelf);
                        }
                    }

                    this.memberRepeater.DataSource = members;
                    this.memberRepeater.DataBind();
                }
            }
        }

        public string GetShowHideCssClass(object show)
        {
            return ((bool)show) ? "" : "hidden";
        }
    }
}
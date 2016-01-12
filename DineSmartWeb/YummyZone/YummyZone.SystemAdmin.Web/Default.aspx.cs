using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Security;
using System.Data.SqlClient;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    public partial class Default : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SystemUser user = LoginHelper.GetIdentityFromAuth(this.Request, false);
            if (user == null)
            {
                this.Response.Redirect("Login.aspx");
                return;
            }

            this.userFriendlyName.InnerText = user.FirstName + " " + user.LastName;

            string redirect = this.Request.Params["redirect"];
            if(redirect != null)
            {
                Guid venueId = Guid.Empty;
                if(Guid.TryParse(redirect, out venueId))
                {
                    if (venueId != Guid.Empty)
                    {
                        Venue theVenue = this.GetVenue(venueId);

                        if (theVenue != null)
                        {
                            string onBehalfIdentity = String.Format(
                                        CultureInfo.InvariantCulture,
                                        "{0}[;]{1}[;]{2}[;]{3}[;]{4}[;]{5}[;]{6} {7}[;]{8}",
                                        user.IsAdmin ? "2" : "1",   // 0
                                        theVenue.GroupId,           // 1
                                        theVenue.ChainId,           // 2
                                        theVenue.Id,                // 3
                                        user.Id,                    // 4
                                        user.EmailAddress,          // 5
                                        user.FirstName,             // 6
                                        user.LastName,              // 7
                                        theVenue.Name);             // 8

                            FormsAuthentication.SetAuthCookie(onBehalfIdentity, false);
                            this.Response.Redirect("onbehalf/Menus.aspx", false);
                            return;
                        }
                    }
                }
            }

            if (!this.IsPostBack)
            {
                string option = "<option value=\"{0}\" {1}>{2}</option>";
                string select = "<select class='inputText timeZoneCombo'>\r\n\t{0}\r\n</select>";

                int defaultTimeZone = Helpers.DefaultTimeZoneIndex;
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                List<int> supportedTimeZones = Helpers.SupportedTimeZoneIndices(timeZones.Count - 1);

                string options = string.Empty;
                for (int x = 0; x < timeZones.Count; x++)
                {
                    if (supportedTimeZones.Contains(x))
                    {
                        string sel = (defaultTimeZone == x) ? "class=\"defaultTimeZoneOption\" selected=\"selected\"" : string.Empty;
                        string opt = String.Format(CultureInfo.InvariantCulture, option, x.ToString(), sel, timeZones[x].DisplayName);
                        options += opt;
                        options += "\r\n";
                    }
                }

                string combo = String.Format(CultureInfo.InvariantCulture, select, options);
                this.timeZoneComboWrp.InnerHtml = combo;
            }
        }

        private Venue GetVenue(Guid venueId)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = Venue.SelectQuery(venueId);
                    command.CommandTimeout = Database.TimeoutSecs;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Venue venue = new Venue();
                            venue.InitFromSqlReader(reader);
                            return venue;
                        }
                    }
                }
            }

            return null;
        }
    }
}
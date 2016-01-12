using System;
using System.Web;

using YummyZone.ObjectModel;
using System.Data.SqlClient;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// ChangePassword httphandler to serve files
    /// </summary>
    public class ChangePassword : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            string oldPassword = this.GetMandatoryString(context, "currentPassword", "Current Password", 100, Source.Form, false);
            string newPassword1 = this.GetMandatoryString(context, "newPassword1", "Current Password", 100, Source.Form, false);
            string newPassword2 = this.GetMandatoryString(context, "newPassword2", "Current Password", 100, Source.Form, false);

            if (newPassword1.Length < 6)
            {
                throw new ArgumentException("New Password is too short");
            }

            if (String.Compare(newPassword1, newPassword2, StringComparison.Ordinal) != 0)
            {
                throw new ArgumentException("New Password pair doesn't match");
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                if (identity.UserType == UserType.Customer)
                {
                    Password pswd = new Password();
                    pswd.UserId = identity.UserId;

                    if (Database.Select(pswd, connection, null, Database.TimeoutSecs))
                    {
                        if (String.Compare(pswd.PasswordText, oldPassword, StringComparison.Ordinal) == 0)
                        {
                            // good
                            pswd.PasswordText = newPassword1;
                            Database.InsertOrUpdate(pswd, connection, null, Database.TimeoutSecs);
                        }
                        else
                        {
                            throw new YummyZoneArgumentException("Wrong password");
                        }
                    }
                    else
                    {
                        // user record exists but there is not any password record
                        throw new YummyZoneArgumentException("Unexpected error: User info couldn't be retrieved");
                    }
                }
                else
                {
                    SystemUser sysUser = new SystemUser();
                    sysUser.Id = identity.UserId;
                    if (Database.Select(sysUser, connection, null, Database.TimeoutSecs))
                    {
                        if (String.Compare(sysUser.UserPassword, oldPassword, StringComparison.Ordinal) == 0)
                        {
                            // good
                            sysUser.UserPassword = newPassword1;
                            Database.InsertOrUpdate(sysUser, connection, null, Database.TimeoutSecs);
                        }
                        else
                        {
                            throw new YummyZoneArgumentException("Wrong password");
                        }
                    }
                    else
                    {
                        // user record exists but there is not any password record
                        throw new YummyZoneArgumentException("Unexpected error: System User info couldn't be retrieved");
                    }
                }
            }

            return 1; // success
        }
    }
}
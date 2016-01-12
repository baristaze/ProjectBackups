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
    /// httphandler
    /// </summary>
    public class ChangeUserStatus : YummyZoneHttpHandler
    {
        private enum Act
        {
            Unknown,
            Delete,
            Disable,
            Enable
        }

        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            // get email
            string email = this.GetMandatoryString(context, "userEmail", "Email Address", 200, Source.Url, false);
            if (!StringHelpers.IsValidEmail(email))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }

            // get action
            Act act = Act.Unknown;
            string action = this.GetMandatoryString(context, "act", "Action type", 10, Source.Url);
            if (String.Compare(action, "delete", StringComparison.OrdinalIgnoreCase) == 0)
            {
                act = Act.Delete;
            }
            else if (String.Compare(action, "disable", StringComparison.OrdinalIgnoreCase) == 0)
            {
                act = Act.Disable;
            }
            else if (String.Compare(action, "enable", StringComparison.OrdinalIgnoreCase) == 0)
            {
                act = Act.Enable;
            }
            else
            {
                throw new YummyZoneArgumentException("Invalid Action Type");
            }
            
            // check to see if user exists
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    Exception e = null;
                    try
                    {
                        this.PerformAction(connection, trans, identity, email, act);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        if (e != null)
                        {
                            trans.Rollback();
                            throw e;
                        }
                    }
                }
            }

            return 0;
        }

        private void PerformAction(SqlConnection connection, SqlTransaction trans, TenantIdentity identity, string email, Act act)
        {
            User user = this.ReadUser(connection, trans, email);
            if (user == null)
            {
                throw new YummyZoneArgumentException("User couldn't be found");
            }

            if (user.GroupId != identity.GroupId)
            {
                throw new YummyZoneException("You don't have permission for this operation");
            }

            if (user.Id == identity.UserId)
            {
                throw new YummyZoneException("You cannot change your membership");
            }

            UserRole role = new UserRole(identity.GroupId);
            role.VenueId = identity.VenueId;
            role.UserId = user.Id;
            if (!Database.Select(role, connection, trans, Database.TimeoutSecs))
            {
                throw new YummyZoneException("User doesn't exist or is not a member");
            }

            if (role.Status == Status.Active && act == Act.Enable)
            {
                return;
            }

            if (role.Status == Status.Disabled && act == Act.Disable)
            {
                return;
            }

            // handle disable/enable
            if (act != Act.Delete)
            {
                role.Status = ((act == Act.Enable) ? Status.Active : Status.Disabled);
                Database.InsertOrUpdate(role, connection, trans, Database.TimeoutSecs);
            }
            else
            {
                // read memberships
                MemberUserList members = MemberUser.Select(connection, trans, identity.GroupId, identity.VenueId, identity.UserId);
                MemberUser memberUser = members[user.Id];
                if (memberUser != null)
                {
                    if (memberUser.CanDelete)
                    {
                        // delete role
                        Database.Delete(role, connection, trans, Database.TimeoutSecs);

                        if (memberUser.MembershipCount <= 1)
                        {
                            // delete password
                            Password pswd = new Password();
                            pswd.UserId = user.Id;
                            Database.Delete(pswd, connection, trans, Database.TimeoutSecs);

                            // delete user
                            Database.Delete(user, connection, trans, Database.TimeoutSecs);
                        }
                    }
                    else
                    {
                        role.Status = Status.Disabled;
                        Database.InsertOrUpdate(role, connection, trans, Database.TimeoutSecs);
                    }
                }
            }
        }

        private User ReadUser(SqlConnection connection, SqlTransaction trans, string email)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = User.SelectByEmailQuery();
                command.Transaction = trans;
                command.CommandTimeout = Database.TimeoutSecs;
                User.AddSqlParameterForEmail(command, email);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            User user = new User();
                            user.InitFromSqlReader(reader);
                            return user;
                        }
                    }
                }
            }

            return null;
        }
    }
}
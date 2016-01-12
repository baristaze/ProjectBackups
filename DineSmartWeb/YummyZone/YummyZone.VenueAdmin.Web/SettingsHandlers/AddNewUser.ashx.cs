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
    public class AddNewUser : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            // get email
            string email = this.GetMandatoryString(context, "userEmail", "Email Address", 200, Source.Form, false);
            if (!StringHelpers.IsValidEmail(email))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }

            // check to see if user exists
            User user = null;
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                user = this.ReadUser(connection, null, email);

                if (user != null)
                {
                    if (user.GroupId != identity.GroupId)
                    {
                        throw new YummyZoneException("This user is already exist and he/she is out of your group.");
                    }

                    if (user.Status != Status.Active)
                    {
                        throw new YummyZoneException("This user is globally disabled; you cannot add it.");
                    }

                    if (user.Id == identity.UserId)
                    {
                        throw new YummyZoneException("User already exists");
                    }

                    UserRole existingRole = new UserRole(identity.GroupId);
                    existingRole.VenueId = identity.VenueId;
                    existingRole.UserId = user.Id;
                    if (Database.Select(existingRole, connection, null, Database.TimeoutSecs))
                    {
                        if (existingRole.Status == Status.Removed)
                        {
                            throw new YummyZoneException("User is marked as 'removed' and he/she cannot be re-added.");
                        }
                        else
                        {
                            throw new YummyZoneException("User already exists");
                        }
                    }
                }                
            }

            // prepare user info
            string temporaryPassword = String.Empty;
            List<IEditable> entities = new List<IEditable>();            
            if (user == null)
            {
                // new user...
                user = new User();
                user.Id = Guid.NewGuid();
                user.GroupId = identity.GroupId;
                user.Status = Status.Active;
                user.CreateTimeUTC = DateTime.UtcNow;
                user.LastUpdateTimeUTC = user.CreateTimeUTC;
                user.EmailAddress = email;
                this.GetOtherFields(context, user, out temporaryPassword);
                entities.Add(user);

                // pswd for the new user
                Password pswd = new Password();
                pswd.UserId = user.Id;
                pswd.PasswordText = temporaryPassword;
                entities.Add(pswd);
            }

            // new role for either new or existing user
            UserRole role = new UserRole(identity.GroupId);
            role.VenueId = identity.VenueId;
            role.UserId = user.Id;
            role.Role = Role.VenueManager;
            role.Status = Status.Active;
            entities.Add(role);

            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            return 1;
        }

        private void GetOtherFields(HttpContext context, User user, out string password)
        {
            password = null;

            string userFirstName = this.GetMandatoryString(context, "userFirstName", "First Name", 100, Source.Form);
            if (userFirstName.IndexOfAny(User.ExcludedCharsInName.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in First Name");
            }

            string userLastName = this.GetMandatoryString(context, "userLastName", "Last Name", 100, Source.Form);
            if (userLastName.IndexOfAny(User.ExcludedCharsInName.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in Last Name");
            }

            string userPhone = this.GetString(context, "userPhone", "Phone Number", 20, Source.Form);
            if (!String.IsNullOrWhiteSpace(userPhone))
            {
                if (!StringHelpers.IsValidPhoneNumber(userPhone))
                {
                    throw new YummyZoneArgumentException("Invalid Phone Number");
                }
            }

            string userPassword = this.GetMandatoryString(context, "userPassword", "Password", 100, Source.Form, false);
            if (userPassword.Length < 6)
            {
                throw new YummyZoneArgumentException("Password is too short.");
            }

            user.FirstName = userFirstName;
            user.LastName = userLastName;
            user.PhoneNumber = userPhone;
            password = userPassword;
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
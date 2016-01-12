using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public class LoginHelper
    {
        public static void CheckLoginInput(string email, string pswd, bool checkPswdLength)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new YummyZoneArgumentException("Please specify email address");
            }

            if (email.Length > 200)
            {
                throw new YummyZoneArgumentException("Email address is too long");
            }

            if (!StringHelpers.IsValidEmail(email))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }

            if (String.IsNullOrWhiteSpace(pswd))
            {
                throw new YummyZoneArgumentException("Please specify password");
            }

            if (checkPswdLength && (pswd.Length > 100))
            {
                throw new YummyZoneArgumentException("Password is too long");
            }
        }

        public static string GetIdentityKey(string userEmail, string userPassword)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                User user = CheckUser(connection, userEmail, userPassword);
                if (user != null)
                {
                    UserRole role = GetUserRoleAtVenue(connection, null, user);
                    if (role == null)
                    {
                        throw new YummyZoneException("Your name is not associated to any venue yet");
                    }

                    // retrieve venue
                    Venue theVenue = new Venue();
                    theVenue.GroupId = role.GroupId;
                    theVenue.Id = role.VenueId;
                    if (!Database.Select(theVenue, connection, null, Database.TimeoutSecs))
                    {
                        throw new YummyZoneException("Venue information couldn't be retrieved");
                    }

                    return String.Format(
                        CultureInfo.InvariantCulture,
                        "0[;]{0}[;]{1}[;]{2}[;]{3}[;]{4}[;]{5} {6}[;]{7}",
                        user.GroupId,       // 0
                        theVenue.ChainId,   // 1
                        theVenue.Id,        // 2
                        user.Id,            // 3
                        user.EmailAddress,  // 4
                        user.FirstName,     // 5
                        user.LastName,      // 6
                        theVenue.Name);     // 7
                }
                else
                {
                    // throw new YummyZoneException("Authentication failed");
                    throw new YummyZoneArgumentException("No user with matching email and password");
                }
            }
        }

        public static UserRole GetUserRoleAtVenue(SqlConnection connection, SqlTransaction transaction, User user)
        {
            UserRole roleTemplate = new UserRole();
            roleTemplate.GroupId = user.GroupId;
            roleTemplate.UserId = user.Id;

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = roleTemplate.SelectForLoginQuery();
                command.Transaction = transaction;
                command.CommandTimeout = Database.TimeoutSecs;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserRole role = new UserRole();
                        role.InitFromSqlReader(reader);
                        return role;
                    }
                }
            }

            return null;
        }

        public static TenantIdentity GetIdentityFromAuth(HttpRequest request, bool throwOnError)
        {
            TenantIdentity identity = _GetIdentityFromAuth(request, false);

            if (identity == null && throwOnError)
            {
                throw new YummyZoneException("Access Denied");
            }

            return identity;
        }

        private static TenantIdentity _GetIdentityFromAuth(HttpRequest request, bool checkNames)
        {
            if (request.IsAuthenticated)
            {
                if (HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity != null &&
                    !String.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    string name = HttpContext.Current.User.Identity.Name;
                    string[] tokens = name.Split(new string[] { "[;]" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens != null && tokens.Length == 8)
                    {
                        TenantIdentity identity = new TenantIdentity();

                        int userType = -1;
                        if (Int32.TryParse(tokens[0], out userType))
                        {
                            if (userType == 0)
                            {
                                identity.UserType = UserType.Customer;
                            }
                            else if (userType == 1)
                            {
                                identity.UserType = UserType.SupportAgent;
                            }
                            else if (userType == 2)
                            {
                                identity.UserType = UserType.SystemAdmin;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }

                        Guid guid;
                        if (Guid.TryParse(tokens[1], out guid))
                        {
                            identity.GroupId = guid;
                        }
                        else
                        {
                            return null;
                        }

                        if (Guid.TryParse(tokens[2], out guid))
                        {
                            identity.ChainId = guid;
                        }
                        else
                        {
                            return null;
                        }

                        if (Guid.TryParse(tokens[3], out guid))
                        {
                            identity.VenueId = guid;
                        }
                        else
                        {
                            return null;
                        }

                        if (Guid.TryParse(tokens[4], out guid))
                        {
                            identity.UserId = guid;
                        }
                        else
                        {
                            return null;
                        }

                        identity.UserEmail = tokens[5];
                        identity.UserFriendlyName = tokens[6];
                        identity.VenueName = tokens[7];

                        if (!identity.Verify(checkNames))
                        {
                            return null;
                        }

                        return identity;
                    }
                }
            }

            return null;
        }

        public static User CheckUser(SqlConnection connection, string userEmail, string userPassword)
        {
            User user = ReadUser(connection, userEmail, userPassword);

            if(user != null)
            {
                // this might throw exception
                CheckPassword(user.Id, connection, userPassword);

                if (user.Status == Status.Active)
                {
                    // you are good if you are here
                    return user;
                }
                else
                {
                    if (user.Status == Status.Disabled)
                    {
                        // throw new YummyZoneException("User is disabled");
                        throw new YummyZoneArgumentException("No user with matching email and password");
                    }
                    else if (user.Status == Status.Removed)
                    {
                        // throw new YummyZoneException("User has marked as deleted");
                        throw new YummyZoneArgumentException("No user with matching email and password");
                    }
                    else
                    {
                        // throw new NotImplementedException("Invalid user state");
                        throw new YummyZoneArgumentException("No user with matching email and password");
                    }
                }
            }
            else
            {
                // user is null
                CheckProvisioningQueue(connection, userEmail, userPassword);
            }

            return null;
        }

        public static User ReadUser(SqlConnection connection, string userEmail, string userPassword)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = User.SelectByEmailQuery();
                command.CommandTimeout = Database.TimeoutSecs;
                User.AddSqlParameterForEmail(command, userEmail);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User();
                        user.InitFromSqlReader(reader);
                        return user;
                    }
                }
            }

            return null;
        }

        public static void CheckProvisioningQueue(SqlConnection connection, string userEmail, string userPassword)
        {
            // check provisioning queue
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = SignupInfo.SelectByEmailQuery();
                command.CommandTimeout = Database.TimeoutSecs;
                SignupInfo.AddSqlParameterForEmail(command, userEmail);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        SignupInfo signupInfo = new SignupInfo();
                        signupInfo.InitFromSqlReader(reader);
                        if (String.Compare(signupInfo.UserPassword, userPassword, StringComparison.Ordinal) == 0)
                        {
                            // it is still in provisioning queue
                            throw new YummyZoneException("Your signup request hasn't been approved yet");
                        }
                        else
                        {
                            // wrong password
                            // throw new YummyZoneException("Email and password don't match [2]");
                            throw new YummyZoneArgumentException("No user with matching email and password");
                        }
                    }
                    else
                    {
                        // we don't have such a user at all
                        // throw new YummyZoneException("There is no such a user");
                        throw new YummyZoneArgumentException("No user with matching email and password");
                    }
                }
            }
        }

        public static void CheckPassword(Guid userId, SqlConnection connection, string userPassword)
        {
            Password pswd = new Password();
            pswd.UserId = userId;

            if (Database.Select(pswd, connection, null, Database.TimeoutSecs))
            {
                if (String.Compare(pswd.PasswordText, userPassword, StringComparison.Ordinal) == 0)
                {
                    // good
                }
                else
                {
                    // wrong password
                    // throw new YummyZoneArgumentException("Email and password don't match");
                    throw new YummyZoneArgumentException("No user with matching email and password");
                }
            }
            else
            {
                // user record exists but there is not any password record
                // throw new YummyZoneArgumentException("Password hasn't been defined yet");
                throw new YummyZoneArgumentException("No user with matching email and password");
            }
        }
    }
}
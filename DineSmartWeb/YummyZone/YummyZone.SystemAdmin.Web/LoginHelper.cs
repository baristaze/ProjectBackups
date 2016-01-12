using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    public class LoginHelper
    {
        public static SystemUser GetIdentityFromAuth(HttpRequest request, bool throwOnError)
        {
            SystemUser identity = _GetIdentityFromAuth(request);

            if (identity == null && throwOnError)
            {
                throw new YummyZoneException("Access Denied");
            }

            return identity;
        }

        private static SystemUser _GetIdentityFromAuth(HttpRequest request)
        {
            if (request.IsAuthenticated)
            {
                if (HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity != null &&
                    !String.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    string name = HttpContext.Current.User.Identity.Name;
                    string[] tokens = name.Split(new string[] { "[;]" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens != null && tokens.Length == 5)
                    {
                        Guid guid;
                        SystemUser identity = new SystemUser();

                        if (Guid.TryParse(tokens[0], out guid))
                        {
                            identity.Id = guid;
                        }
                        else
                        {
                            return null;
                        }

                        if (tokens[1] == "0") 
                        {
                            identity.IsAdmin = false;
                        }
                        else if (tokens[1] == "1")
                        {
                            identity.IsAdmin = true;
                        }
                        else
                        {
                            return null;
                        }

                        identity.EmailAddress = tokens[2];
                        identity.FirstName = tokens[3];
                        identity.LastName = tokens[4];

                        return identity;
                    }
                }
            }

            return null;
        }

        public static string GetIdentityKey(string userEmail, string userPassword)
        {
            SystemUser user = ReadSystemUser(userEmail, userPassword);
            if (user != null)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}[;]{1}[;]{2}[;]{3}[;]{4}",
                    user.Id,
                    user.IsAdmin ? "1" : "0",
                    user.EmailAddress,
                    user.FirstName,
                    user.LastName);
            }

            return null;
        }

        private static SystemUser ReadSystemUser(string userEmail, string userPassword)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = SystemUser.SelectByEmailAndPasswordQuery();
                    command.CommandTimeout = Database.TimeoutSecs;
                    SystemUser.AddSqlParametersForEmailAndPassword(command, userEmail, userPassword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SystemUser user = new SystemUser();
                            user.InitFromSqlReader(reader);
                            return user;
                        }
                    }
                }
            }

            return null;
        }

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
    }
}
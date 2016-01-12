using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    public class UserNameAuthenticator : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(OnBeginRequest);
            application.AuthenticateRequest += new EventHandler(this.OnAuthenticateRequest);
            application.EndRequest += new EventHandler(this.OnEndRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            
            string svcName = "/YummyZoneWebService.svc";
            string svcNamePlaceholder = Helpers.AbsPathPrefix;
            string absPath = app.Request.Url.AbsolutePath.ToString();

            Logger.LogAsVerbose("OnBeginRequest", Guid.Empty, "AbsolutePath: '{0}'", absPath);

            if(absPath.StartsWith(svcNamePlaceholder, StringComparison.InvariantCultureIgnoreCase))
            {
                string query = app.Request.Url.Query;
                if(!String.IsNullOrWhiteSpace(query))
                {
                    query = query.TrimStart('?');
                    query = "format=plist&" + query;
                }
                else
                {
                    query = "format=plist";
                }

                absPath = absPath.Substring(svcNamePlaceholder.Length);
                app.Context.RewritePath("~" + svcName, absPath, query);
            }
        }

        private void OnAuthenticateRequest(object source, EventArgs eventArgs)
        {
            HttpApplication app = (HttpApplication)source;

            // is this a call for sign-up?
            string path = app.Request.Url.AbsolutePath;
            
            Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "Requested URL: '{0}'", app.Request.Url.ToString());
            Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "AbsolutePath: '{0}'", path);

            path = path.Trim('/', ' ');
            string[] tokens = path.Split('/');
            if (tokens.Length > 0)
            {
                string[] allowedCalls = new string[] { "Signup", "Signin", "SendPasswordReminder" };
                foreach (string allowedCall in allowedCalls)
                {
                    if (String.Compare(tokens[tokens.Length - 1], allowedCall, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        // signup call... no need to have auth cookie.
                        Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "AllowedCall: '{0}'", allowedCall);
                        return;
                    }
                }
            }
            
            //the Authorization header is checked if present
            string authHeader = app.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                string authStr = app.Request.Headers["Authorization"];

                if (authStr == null || authStr.Length == 0)
                {
                    // No credentials; anonymous request
                    Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "No credentials... Anonymous request", null);
                    return;
                }

                authStr = authStr.Trim();
                if (authStr.IndexOf("Basic ", 0, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // header is not correct...we'll pass it along and 
                    // assume someone else will handle it
                    Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "Basic Auth is requested", null);
                    return;
                }

                string encodedCredentials = authStr.Substring(6);
                string s = OM.AsciiBase64Codex.Decode(encodedCredentials);

                string[] userPass = s.Split(new char[] { ':' });
                string username = userPass[0];
                string password = userPass[1];

                // the user is validated against a database
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();

                    OM.Diner diner = DinerUserHelper.GetUser(connection, username, password);
                    if (diner != null && diner.Status == OM.Status.Active)
                    {
                        string[] roles = new string[] { "SmartPhoneAccess" };
                        app.Context.User = new GenericPrincipal(new GenericIdentity(diner.ToString()), roles);
                        Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "Authenticated User: '{0}'", username);
                    }
                    else
                    {
                        Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "Denying Access for User: '{0}'", username);
                        DenyAccess(app);
                        return;
                    }
                }
            }
            else
            {
                Logger.LogAsVerbose("OnAuthenticateRequest", Guid.Empty, "There is no authorization header", null);
                app.Response.StatusCode = 401;
                app.Response.End();
            }
        }

        private void OnEndRequest(object source, EventArgs eventArgs)
        {
            //the authorization header is not present
            //the status of response is set to 401 and it ended
            //the end request will check if it is 401 and add
            //the authentication header so the client knows
            //it needs to send credentials to authenticate
            if (HttpContext.Current.Response.StatusCode == 401)
            {
                HttpContext context = HttpContext.Current;
                context.Response.StatusCode = 401;
                context.Response.AddHeader("WWW-Authenticate", "Basic Realm");
            }
        }

        private void DenyAccess(HttpApplication app)
        {
            app.Response.StatusCode = 401;
            app.Response.StatusDescription = "Access Denied";

            // Write to response stream as well, to give user visual 
            // indication of error during development
            app.Response.Write("401 Access Denied");

            app.CompleteRequest();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Shebeke.ObjectModel;
using System.Globalization;
using System.Diagnostics;

namespace Shebeke.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // init cache values
            try
            {
                Config config = new Config();
                config.Init();
                Reaction.InitAll(config.DBaseConnectionString);
            }
            catch (Exception ex)
            {
                try
                {
                    string msg = "Reaction types couldn't be initialized. Exception: " + ex.ToString();

                    string errorLog = String.Format(
                                CultureInfo.InvariantCulture,
                                "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                                "Global.asax.cs",
                                "Application_Start",
                                "InitReactions",
                                0,
                                0,
                                msg);

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }
                catch { }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
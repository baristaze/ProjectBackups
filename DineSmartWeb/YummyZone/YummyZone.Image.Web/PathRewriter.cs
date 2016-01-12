using System;
using System.Web;
using System.Configuration;

namespace YummyZone.Image.Web
{
    public class PathRewriter : IHttpModule
    {
        private static string AbsPathPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["AbsPathPrefix"];
            }
        }

        private static string AbsPathPrefixLogo
        {
            get
            {
                return ConfigurationManager.AppSettings["AbsPathPrefixLogo"];
            }
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;            
            string absPath = app.Request.Url.AbsolutePath.ToString();
            Logger.LogAsVerbose("OnBeginRequest", Guid.Empty, "AbsolutePath: '{0}'", absPath);

            if (!this.TryRewrite(app, absPath, AbsPathPrefixLogo, "/Logo.ashx"))
            {
                this.TryRewrite(app, absPath, AbsPathPrefix, "/File.ashx");
            }
        }

        private bool TryRewrite(HttpApplication app, string absPath, string svcNamePlaceholder, string svcName)
        {
            if (absPath.StartsWith(svcNamePlaceholder, StringComparison.InvariantCultureIgnoreCase))
            {
                string query = absPath.Substring(svcNamePlaceholder.Length);
                if (!String.IsNullOrWhiteSpace(query))
                {
                    query = "fid=" + query;
                }

                app.Context.RewritePath("~" + svcName, "", query);
                return true;
            }
            else
            {
                return false;
            } 
        }
    }
}

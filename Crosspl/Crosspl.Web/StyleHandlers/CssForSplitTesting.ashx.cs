using System;
using System.Collections.Generic;
using System.Web;

using Crosspl.ObjectModel;
using System.Data.SqlClient;

namespace Crosspl.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class CssForSplitTesting : CrossplHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context, out UserAuthInfo user)
        {
            context.Response.ContentType = "text/css";

            Config config = new Config();
            config.Init();

            user = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // determine the split id
            string splitText = context.Request.Params["split"];
            int theSplitId = Splitter.DetermineSplitId(user, -1, splitText);

            // sanitize the input
            string sections = context.Request.Params["sections"];
            sections = Splitter.SanitizeSectionFilters(sections);

            // read split info from db
            CssSplitterList cssList = new CssSplitterList();
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                cssList = CssSplitter.Read(connection, null, theSplitId, sections);
            }

            string totalCss = String.Empty;
            string appPath = context.Request.ApplicationPath.TrimEnd('/');
            if (cssList != null && cssList.Count > 0)
            {
                IEnumerable<string> cssClasses = cssList.GetUniqueCssClassNames();
                foreach (string cssClass in cssClasses)
                {
                    string css = String.Empty;
                    IEnumerable<CssSplitter> items = cssList.GetByCssClassName(cssClass);
                    foreach (CssSplitter item in items)
                    {
                        item.ReplacePlaceholders("__APP_PATH__", appPath);

                        css += item.ComputedValue + ";";
                    }
                    css = "." + cssClass + "{" + css + "}";
                    totalCss += css + "\n";
                }
            }

            return totalCss;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using Shebeke.ObjectModel;

namespace Shebeke.Web
{
    public partial class ImageRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string assetIdText = this.Request.QueryString["assetId"];
            if (!String.IsNullOrWhiteSpace(assetIdText))
            {
                long assetId = -1;
                if (long.TryParse(assetIdText, out assetId))
                {
                    if (assetId > 0)
                    { 
                        Config config = new Config();
                        config.Init();

                        // get file meta data
                        using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                        {
                            conn.Open();
                            ImageFile imageFile = ImageFile.ReadFromDBase(conn, null, assetId, false, 0);
                            if(imageFile != null && imageFile.AssetType == AssetType.Entry)
                            {
                                this.Response.Redirect(imageFile.CloudUrl);
                                return;
                            }
                        }
                    }
                }
            }

            this.Response.StatusCode = 404;
            this.Response.Status = "404 Not Found";
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;

using YummyZone.ObjectModel;

namespace YummyZone.Image.Web
{
    /// <summary>
    /// FileDownload httphandler to serve files
    /// </summary>
    public class BaseFileHttpHandler<T> : IHttpHandler where T:AssetImage, new()
    {
        internal static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            }
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            bool fileProvided = false;
            Guid imageId = Guid.Empty;

            try
            {
                string fid = context.Request.Params["fid"];
                if (!String.IsNullOrWhiteSpace(fid))
                {
                    Guid fileId = Guid.Empty;
                    if (Guid.TryParse(fid, out fileId))
                    {
                        if (fileId != Guid.Empty)
                        {
                            imageId = fileId;
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = connection.CreateCommand())
                                {
                                    command.CommandText = (new T()).SelectQuery(fileId);
                                    command.CommandTimeout = Database.TimeoutSecsForFile;

                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            T image = new T();
                                            image.InitFromSqlReader(reader);

                                            context.Response.ContentType = image.ContentType;
                                            context.Response.AddHeader("Content-Length", image.ContentLength.ToString());
                                            context.Response.BinaryWrite(image.Data);

                                            fileProvided = true;

                                            context.Response.End();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!fileProvided)
                {
                    Logger.LogException(ex, "ProcessRequest", imageId);
                }
            }
            finally
            {
                if (!fileProvided)
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                }
            }
        }
    }
}
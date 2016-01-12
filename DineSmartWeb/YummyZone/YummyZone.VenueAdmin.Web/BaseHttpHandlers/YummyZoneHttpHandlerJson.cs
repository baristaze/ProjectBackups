using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    [DataContract]
    public class BaseJsonResponse
    {
        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    public abstract class YummyZoneHttpHandlerJson<TResponse> : YummyZoneHttpHandler where TResponse : BaseJsonResponse, new()
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoServerCaching();
            context.Response.Expires = -10000;

            TResponse response = new TResponse();

            try
            {
                response = (TResponse)this.ProcessWebRequest(context);
                response.ErrorCode = 0;
                response.ErrorMessage = "Success";
            }
            catch (YummyZoneArgumentException ex)
            {
                response.ErrorCode = 1;
                response.ErrorMessage = ex.Message;
            }
            catch (YummyZoneException ex)
            {
                response.ErrorCode = 2;
                response.ErrorMessage = ex.Message;
            }
            catch (SqlException)
            {
                response.ErrorCode = 3;
                response.ErrorMessage = "Couldn't read from database.";
            }
            catch (Exception)
            {
                response.ErrorCode = 4;
                response.ErrorMessage = "Unknown exception occurred.";
            }

            // Serialize to JSON
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TResponse));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, response);
                string json = Encoding.UTF8.GetString(ms.ToArray());

                context.Response.ContentType = "application/json";
                context.Response.Write(json);
                context.Response.End();
            }
        }
    }
}
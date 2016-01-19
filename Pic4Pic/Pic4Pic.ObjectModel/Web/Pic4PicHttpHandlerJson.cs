using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public abstract class Pic4PicHttpHandlerJson<TResponse> : Pic4PicHttpHandler where TResponse : BaseResponse, new()
    {
        protected virtual string GetResponseContentType() 
        {
            // search your javascript files with "sanitizeJsonResponse"
            // Safari and Firefox encapsulates the json data with a "<pre></pre>" tag.
            return "application/json";
        }

        public override void ProcessRequest(HttpContext context)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string methodName = this.GetType().Name; // handler name is a better option here

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoServerCaching();
            context.Response.Expires = -10000;

            TResponse response = new TResponse();
            UserAuthInfo user = null;

            try
            {
                response = (TResponse)this.ProcessWebRequest(context, out user);
                response.ErrorCode = 0;
                response.ErrorMessage = "Success";
            }
            catch (Pic4PicArgumentException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                response.ErrorCode = 1;
                response.ErrorMessage = ex.Message;
            }
            catch (Pic4PicException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                response.ErrorCode = 2;
                response.ErrorMessage = ex.Message;

                if (ex is Pic4PicAuthException)
                {
                    response.ErrorCode = 401;
                    response.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                response.ErrorCode = 3;
                response.ErrorMessage = "Unexpected database error!";
            }
            catch (Exception ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                response.ErrorCode = 4;
                response.ErrorMessage = "Unexpected error!";
            }

            // Serialize to JSON
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TResponse));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, response);
                string json = Encoding.UTF8.GetString(ms.ToArray());

                context.Response.ContentType = this.GetResponseContentType();
                context.Response.Write(json);
            }

            // set metrics
            stopWatch.Stop();
            TimeSpan elapsedTime = stopWatch.Elapsed;

            Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add("elapsed", elapsedTime.TotalSeconds.ToString("F2"))
                    .LogMetric();

            context.Response.End();
        }
    }
}
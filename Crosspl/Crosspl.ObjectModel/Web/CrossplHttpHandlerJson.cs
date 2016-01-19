using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public abstract class CrossplHttpHandlerJson<TResponse> : CrossplHttpHandler where TResponse : BaseResponse, new()
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
            catch (CrossplArgumentException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "CrossplHttpHandlerJson.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                response.ErrorCode = 1;
                response.ErrorMessage = ex.Message;
            }
            catch (CrossplException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "CrossplHttpHandlerJson.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                response.ErrorCode = 2;
                response.ErrorMessage = ex.Message;

                if (ex is CrossplAuthException)
                {
                    response.ErrorCode = 401;
                    response.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "CrossplHttpHandlerJson.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                response.ErrorCode = 3;
                response.ErrorMessage = "Unexpected Database Error";
            }
            catch (Exception ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "CrossplHttpHandlerJson.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                response.ErrorCode = 4;
                response.ErrorMessage = "Unexpected Internal Error";
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
            string metricLog = String.Format(
                CultureInfo.InvariantCulture,
                // "[Version=1];[MetricName=ElapsedSeconds];[MethodName={0}];[ElapsedSeconds={1}]",
                "[Version=2];[MetricName=ElapsedSeconds];[MethodName={0}];[ElapsedSeconds={1}];[User={2}];[Split={3}]",
                methodName,
                elapsedTime.TotalSeconds.ToString("F2"),
                user == null ? 0 : user.UserId,
                user == null ? 0 : user.SplitId);

            Trace.WriteLine(metricLog, LogCategory.Metric);

            context.Response.End();
        }
    }
}
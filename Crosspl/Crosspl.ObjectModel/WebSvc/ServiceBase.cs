using System;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;

namespace Crosspl.ObjectModel
{
    public class ServiceBase
    {
        public delegate R Target<R>(out UserAuthInfo user);
        public delegate R Target<R, P>(P input, out UserAuthInfo user);

        protected R SafeExecute<R>(string methodName, Target<R> target, out UserAuthInfo user) where R : BaseResponse, new()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            R result = null;
            user = null;

            try
            {
                result = target(out user);
            }
            catch (CrossplArgumentException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 100;
                result.ErrorMessage = ex.Message;
            }
            catch (CrossplException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 101;
                result.ErrorMessage = ex.Message;

                if (ex is CrossplAuthException)
                {
                    result.ErrorCode = 401;
                    result.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 102;
                result.ErrorMessage = "Unexpected Database Error";
            }
            catch (Exception ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 103;
                result.ErrorMessage = "Unexpected Internal Error";
            }
            finally
            {
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
            }

            return result;
        }

        protected R SafeExecute<R, P>(string methodName, P input, Target<R, P> target, out UserAuthInfo user) where R : BaseResponse, new()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            R result = null;
            user = null;

            try
            {
                result = target(input, out user);
            }
            catch (CrossplArgumentException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 100;
                result.ErrorMessage = ex.Message;
            }
            catch (CrossplException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 101;
                result.ErrorMessage = ex.Message;

                if (ex is CrossplAuthException)
                {
                    result.ErrorCode = 401;
                    result.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 102;
                result.ErrorMessage = "Unexpected Database Error";
            }
            catch (Exception ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ServiceBase.cs",
                            methodName,
                            "SafeExecuteInvoke",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                result = new R();
                result.ErrorCode = 103;
                result.ErrorMessage = "Unexpected Internal Error";
            }
            finally
            {
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
            }

            return result;
        }

        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV)
        {
            return GetUserInfoFromContent(secretKey, secretIV, true);
        }

        /// <summary>
        /// Gets the user info or throws exception
        /// </summary>
        /// <returns>UserAuthInfo</returns>
        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV, bool throwOnNoAuth)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["XAuthToken"];
            if (authCookie != null && !String.IsNullOrWhiteSpace(authCookie.Value))
            {
                string token = authCookie.Value;
                string decrypt = Crypto.DecryptAES(token, secretKey, secretIV);
                UserAuthInfo userInfo = UserAuthInfo.Parse(decrypt);
                return userInfo;
            }

            if (!throwOnNoAuth)
            {
                return new UserAuthInfo()
                {
                    UserId = 0,
                    UserType = UserType.Guest,
                    FirstName = "Guest",
                    PhotoUrl = HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Images/user.png",
                };
            }

            throw new CrossplAuthException("Access Denied");
        }
    }
}

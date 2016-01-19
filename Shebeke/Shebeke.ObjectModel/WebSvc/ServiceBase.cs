using System;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;

namespace Shebeke.ObjectModel
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
            catch (ShebekeArgumentException ex)
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
            catch (ShebekeException ex)
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

                if (ex is ShebekeAuthException)
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
                result.ErrorMessage = "Beklenmeyen veritabanı hatası! Oluyor bazen.";
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
                result.ErrorMessage = "Beklenmeyen bir hata oluştu. Sorun bizde!";
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
            catch (ShebekeArgumentException ex)
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
            catch (ShebekeException ex)
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

                if (ex is ShebekeAuthException)
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
                result.ErrorMessage = "Beklenmeyen veritabanı hatası! Oluyor bazen.";
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
                result.ErrorMessage = "Beklenmeyen bir hata oluştu. Sorun bizde!";
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
            UserAuthInfo theUser = new UserAuthInfo()
            {
                UserId = 0,
                UserType = UserType.Guest,
                FirstName = "Misafir",
                PhotoUrl = HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Images/user.png",
            };

            Exception theException = null;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["XAuthToken"];
            if (authCookie != null && !String.IsNullOrWhiteSpace(authCookie.Value))
            {
                string token = authCookie.Value;
                string decrypt = Crypto.DecryptAES(token, secretKey, secretIV);
                try
                {
                    theUser = UserAuthInfo.Parse(decrypt);
                }
                catch (ShebekeArgumentException ex)
                {
                    theException = ex;
                }

                if (theUser.UserId > 0)
                {
                    string blockedUserCacheKey = "BlockedUser_" + theUser.UserId.ToString();
                    if (CacheHelper.GetOrDefault<int>(CacheHelper.CacheName_LongLastingObjects, blockedUserCacheKey) > 0)
                    {
                        theException = new ShebekeAuthException(
                            "Hesabınız geçici bir süre bloke edilmiştir. Lütfen sistem yöneticisine (contact@shebeke.net) başvurun.");
                    }
                }
            }
            else
            {
                theException = new ShebekeAuthException("Erişim hakkın yok, veya onun gibi bişey!");
            }

            if (theException == null)
            {
                String metricLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[MetricName=AuthenticatedCall];[Source=Service];[UserId={0}];[FacebookUserId={1}]",
                    theUser.UserId,
                    theUser.OAuthUserId);

                System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);

                return theUser;
            }
            else if (!throwOnNoAuth)
            {
                String metricLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[MetricName=AnonymousCall];[Source=Service];[UserId={0}];[FacebookUserId={1}]",
                    0,
                    "");

                System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);

                return theUser; // the guest
            }

            throw theException;
        }

        protected void LogUserActivity(SqlConnection connection, SqlTransaction trans, long userId, UserActivityType activity)
        {
            UserLogActivity act = new UserLogActivity(userId, activity);

            try
            {
                act.LogUserActivityOnDBase(connection, trans);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected void CheckDenialOfService(
            SqlConnection connection,
            SqlTransaction trans,
            long userId,
            UserActivityType activity)
        {
            int timeWindow = 1 * 24 * 60 * 60; // 1 day
            int limit = UserLogActivity.GetDefaultLimit(activity);
            CheckDenialOfService(connection, trans, userId, activity, timeWindow, limit);
        }

        protected void CheckDenialOfService(
            SqlConnection connection,
            SqlTransaction trans,
            long userId,
            UserActivityType activity,
            int timeWindowInSeconds,
            int activityLimit)
        {
            int result = 0;

            string blockedUserCacheKey = "BlockedUser_" + userId.ToString();
            if (CacheHelper.GetOrDefault<int>(CacheHelper.CacheName_LongLastingObjects, blockedUserCacheKey) > 0)
            {
                // blocked
                result = -1;
            }
            else
            {
                try
                {
                    UserLogActivity act = new UserLogActivity(userId, activity);
                    result = act.CheckUserActivityOnDBase(connection, trans, timeWindowInSeconds, activityLimit);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            if (result < 0)
            {
                CacheHelper.Put(CacheHelper.CacheName_LongLastingObjects, blockedUserCacheKey, 1);
                throw new ShebekeArgumentException("Günlük aktivite limitinizi doldurdunuz. Hesabınız geçici olarak bloke edildi. Lütfen sistem yöneticisine (contact@shebeke.net) başvurun!");
            }
        }
    }
}

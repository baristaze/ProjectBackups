using System;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;
using System.ServiceModel.Web;

namespace Pic4Pic.ObjectModel
{
    public class ServiceBase
    {
        public delegate R Target<R>(out UserAuthInfo user);
        public delegate R Target<R, P>(P input, out UserAuthInfo user);

        protected R SafeExecute<R>(Target<R> target, out UserAuthInfo user) where R : BaseResponse, new()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            R result = null;
            user = null;

            try
            {
                result = target(out user);
            }
            catch (Pic4PicArgumentException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 100;
                result.ErrorMessage = ex.Message;
            }
            catch (Pic4PicException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 101;
                result.ErrorMessage = ex.Message;

                if (ex is Pic4PicAuthException)
                {
                    result.ErrorCode = 401;
                    result.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 102;
                result.ErrorMessage = "Something went wrong in data layer. Please try again later!";
            }
            catch (Exception ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 103;
                result.ErrorMessage = "Something went wrong. Please try again later!";
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan elapsedTime = stopWatch.Elapsed;

                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add("elapsed", elapsedTime.TotalSeconds.ToString("F2"))
                    .LogMetric();
            }

            return result;
        }

        protected R SafeExecute<R, P>(P input, Target<R, P> target, out UserAuthInfo user) where R : BaseResponse, new()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            R result = null;
            user = null;

            try
            {
                if (input == null) 
                {
                    throw new Pic4PicException("Input is null. Check Content-Type, request body and/or query parameters.");
                }

                result = target(input, out user);
            }
            catch (Pic4PicArgumentException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 100;
                result.ErrorMessage = ex.Message;
            }
            catch (Pic4PicException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 101;
                result.ErrorMessage = ex.Message;

                if (ex is Pic4PicAuthException)
                {
                    result.ErrorCode = 401;
                    result.NeedsRelogin = true;
                }
            }
            catch (SqlException ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 102;
                result.ErrorMessage = "Something went wrong in data layer. Please try again later!";
            }
            catch (Exception ex)
            {
                Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                result = new R();
                result.ErrorCode = 103;
                result.ErrorMessage = "Something went wrong! Please try again later.";
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan elapsedTime = stopWatch.Elapsed;
                String callerMethod = Logger.GetCallerMethod(true);
                if (callerMethod == "LogClientTraces")
                {
                    Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add("elapsed", elapsedTime.TotalSeconds.ToString("F2"))
                    .LogVerbose();
                }
                else 
                {
                    Logger.bag(true)
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add("elapsed", elapsedTime.TotalSeconds.ToString("F2"))
                    .LogMetric();
                }
            }

            return result;
        }

        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV)
        {
            return GetUserInfoFromContent(secretKey, secretIV, true);
        }

        protected string GetRequestHeaderValue(string name)
        {
            return WebOperationContext.Current.IncomingRequest.Headers[name];
        }

        /// <summary>
        /// Gets the user info or throws exception
        /// </summary>
        /// <returns>UserAuthInfo</returns>
        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV, bool throwOnNoAuth)
        {
            Exception theException = null;
            UserAuthInfo theUser = UserAuthInfo.CreateDefault();
            string token = this.GetRequestHeaderValue("XAuthToken");
            if (!String.IsNullOrWhiteSpace(token))
            {
                string decrypt = Crypto.DecryptAES(token, secretKey, secretIV);
                try
                {
                    theUser = UserAuthInfo.Parse(decrypt);
                }
                catch (Pic4PicArgumentException ex)
                {
                    theException = ex;
                }

                /*
                if (!theUser.IsDefaultUser)
                {
                    string blockedUserCacheKey = "BlockedUser_" + theUser.UserId.ToString();
                    if (CacheHelper.GetOrDefault<int>(CacheHelper.CacheName_LongLastingObjects, blockedUserCacheKey) > 0)
                    {
                        theException = new Pic4PicAuthException(
                            "Your account is temporarily blocked. Please contact to [contact@pic4pic.net]");
                    }
                }
                */ 
            }
            else
            {
                theException = new Pic4PicAuthException("Access denied!");
            }

            if (theException == null)
            {
                /*
                Logger.bag()
                    .Add("authentication", "1")
                    .LogMetric();
                */

                return theUser;
            }
            else if (!throwOnNoAuth)
            {
                /*
                Logger.bag()
                    .Add("authentication", "0")
                    .LogMetric();
                */

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
                throw new Pic4PicArgumentException("You have reached your daily limit. Your account is temporarily blocked. Please contact to [contact@pic4pic.net]");
            }
        }
    }
}

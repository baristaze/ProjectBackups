using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Diagnostics;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public abstract class Pic4PicHttpHandler : IHttpHandler
    {
        protected enum Source
        {
            Url,
            Form
        }

        public virtual bool IsReusable { get { return false; } }
        
        public virtual void ProcessRequest(HttpContext context)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string methodName = this.GetType().Name; // handler name is a better option here

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoServerCaching();
            context.Response.Expires = -10000;
            UserAuthInfo user = null;

            try
            {
                object o = ProcessWebRequest(context, out user);
                OnSuccess(context, o);
            }
            catch (Pic4PicArgumentException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                context.Response.Write(ex.Message);
            }
            catch (Pic4PicException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                context.Response.Write(ex.Message);

                if (ex is Pic4PicAuthException)
                {
                    context.Response.Write("Access Denied");
                }
            }
            catch (SqlException ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                context.Response.Write("Unexpected database error!");
            }
            catch (Exception ex)
            {
                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add(ex)
                    .LogError();

                context.Response.Write("Unexpected error!");
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan elapsedTime = stopWatch.Elapsed;

                Logger.bag()
                    .Add("userid", (user == null ? Guid.Empty : user.UserId).ToString())
                    .Add("split", (user == null ? 0 : user.SplitId).ToString())
                    .Add("elapsed", elapsedTime.TotalSeconds.ToString("F2"))
                    .LogMetric();
            }

            context.Response.End();
        }

        protected abstract object ProcessWebRequest(HttpContext context, out UserAuthInfo user);

        protected virtual void OnSuccess(HttpContext context, object result) 
        {
            context.Response.Write(result.ToString());
        }

        /// <summary>
        /// Gets the user info or throws exception
        /// </summary>
        /// <returns>UserAuthInfo</returns>
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
            Exception theException = null;
            UserAuthInfo theUser = UserAuthInfo.CreateDefault();
            string token = HttpContext.Current.Request.Headers["XAuthToken"];
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
                if(!theUser.IsDefaultUser)
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

        protected Guid GetGuid(HttpContext context, string paramName, string paramDescription, Source source)
        {
            string guidValueAsStr = null;
            if (source == Source.Url)
            {
                guidValueAsStr = context.Request.Params[paramName];
            }
            else
            {
                guidValueAsStr = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(guidValueAsStr))
            {
                return Guid.Empty;
            }

            Guid guidValue = Guid.Empty;
            guidValueAsStr = guidValueAsStr.Trim();
            if (!Guid.TryParse(guidValueAsStr, out guidValue))
            {
                throw new Pic4PicArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
            }
            
            return guidValue;
        }

        protected Guid GetMandatoryGuid(HttpContext context, string paramName, string paramDescription, Source source)
        {
            string guidValueAsStr = null;
            if (source == Source.Url)
            {
                guidValueAsStr = context.Request.Params[paramName];
            }
            else
            {
                guidValueAsStr = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(guidValueAsStr))
            {
                throw new Pic4PicArgumentException(paramDescription + " hasn't been provided", paramName);
            }

            Guid guidValue = Guid.Empty;
            guidValueAsStr = guidValueAsStr.Trim();
            if (!Guid.TryParse(guidValueAsStr, out guidValue))
            {
                throw new Pic4PicArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
            }

            if (guidValue == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
            }

            return guidValue;
        }

        protected String GetString(HttpContext context, string paramName, string paramDescription, int maxLength, Source source)
        {
            string name = null;
            if (source == Source.Url)
            {
                name = context.Request.Params[paramName];
            }
            else
            {
                name = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            name = name.Trim();
            if (name.Length > maxLength)
            {
                throw new Pic4PicArgumentException(paramDescription + " is too long", paramName);
            }

            return StringHelpers.UppercaseFirst(name);
        }

        protected String GetMandatoryString(HttpContext context, string paramName, string paramDescription, int maxLength, Source source)
        {
            return GetMandatoryString(context, paramName, paramDescription, maxLength, source, true);
        }

        protected String GetMandatoryString(HttpContext context, string paramName, string paramDescription, int maxLength, Source source, bool upperCaseFirstChar)
        {
            string name = null;
            if (source == Source.Url)
            {
                name = context.Request.Params[paramName];
            }
            else
            {
                name = context.Request.Form[paramName];
            }
            
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new Pic4PicArgumentException(paramDescription + " cannot be empty", paramName);
            }

            name = name.Trim();
            if (name.Length > maxLength)
            {
                throw new Pic4PicArgumentException(paramDescription + " is too long", paramName);
            }

            if (upperCaseFirstChar)
            {
                name = StringHelpers.UppercaseFirst(name);
            }
            return name;
        }

        protected int GetMandatoryInt(HttpContext context, string paramName, string paramDescription, int min, int max, Source source)
        {
            string integerValueAsString = null;
            if (source == Source.Url)
            {
                integerValueAsString = context.Request.Params[paramName];
            }
            else
            {
                integerValueAsString = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(integerValueAsString))
            {
                throw new Pic4PicArgumentException(paramDescription + " cannot be empty", paramName);
            }

            integerValueAsString = integerValueAsString.Trim();
            
            int integerValue;
            if (!Int32.TryParse(integerValueAsString, out integerValue))
            {
                throw new Pic4PicArgumentException(paramDescription + " is not an integer", paramName);
            }

            if (integerValue < min || integerValue > max)
            {
                throw new Pic4PicArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
            }

            return integerValue;
        }

        protected int GetIntOrDefault(HttpContext context, string paramName, string paramDescription, int min, int max, int defaultVal, Source source)
        {
            string integerValueAsString = null;
            if (source == Source.Url)
            {
                integerValueAsString = context.Request.Params[paramName];
            }
            else
            {
                integerValueAsString = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(integerValueAsString))
            {
                return defaultVal;
            }

            integerValueAsString = integerValueAsString.Trim();

            int integerValue;
            if (!Int32.TryParse(integerValueAsString, out integerValue))
            {
                return defaultVal;
            }

            if (integerValue < min) 
            {
                return min;
            }

            if (integerValue > max)
            {
                return max;
            }

            return integerValue;
        }

        protected long GetMandatoryLong(HttpContext context, string paramName, string paramDescription, long min, long max, Source source)
        {
            string numberAsString = null;
            if (source == Source.Url)
            {
                numberAsString = context.Request.Params[paramName];
            }
            else
            {
                numberAsString = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(numberAsString))
            {
                throw new Pic4PicArgumentException(paramDescription + " cannot be empty", paramName);
            }

            numberAsString = numberAsString.Trim();

            long number;
            if (!Int64.TryParse(numberAsString, out number))
            {
                throw new Pic4PicArgumentException(paramDescription + " is not an Int64", paramName);
            }

            if (number < min || number > max)
            {
                throw new Pic4PicArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
            }

            return number;
        }

        protected DateTime GetMandatoryDateTime(HttpContext context, string paramName, string paramDescription, Source source)
        {
            string dateTimeValueAsString = null;
            if (source == Source.Url)
            {
                dateTimeValueAsString = context.Request.Params[paramName];
            }
            else
            {
                dateTimeValueAsString = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(dateTimeValueAsString))
            {
                throw new Pic4PicArgumentException(paramDescription + " cannot be empty", paramName);
            }

            dateTimeValueAsString = dateTimeValueAsString.Trim();

            DateTime dateTimeValue;
            if (!DateTime.TryParse(dateTimeValueAsString, out dateTimeValue))
            {
                throw new Pic4PicArgumentException(paramDescription + " is not a date-time", paramName);
            }

            return dateTimeValue;
        }

        protected bool GetDeleteOrDisableAction(HttpContext context, Source source)
        {
            string action = this.GetMandatoryString(context, "act", "Action type", 10, source);
            
            bool? isDelete = null;
            action = action.Trim();
            if (String.Compare(action, "delete", StringComparison.OrdinalIgnoreCase) == 0)
            {
                isDelete = true;
            }
            else if (String.Compare(action, "disable", StringComparison.OrdinalIgnoreCase) == 0)
            {
                isDelete = false;
            }

            if (!isDelete.HasValue)
            {
                throw new Pic4PicArgumentException("Invalid Action Type: " + action, "act");
            }

            return isDelete.Value;
        }

        protected List<Guid> GetMandatoryGuidList(HttpContext context, string paramName, string paramDescription)
        {
            return this.GetMandatoryGuidList(context, paramName, paramDescription, Source.Url);
        }

        protected List<Guid> GetMandatoryGuidList(HttpContext context, string paramName, string paramDescription, Source source)
        {
            string concatenatedGuids = null;
            if (source == Source.Url)
            {
                concatenatedGuids = context.Request.Params[paramName];
            }
            else
            {
                concatenatedGuids = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(concatenatedGuids))
            {
                string msg = "There hasn't been specified any " + paramDescription + " as an input";
                throw new Pic4PicArgumentException(msg, paramName);
            }

            concatenatedGuids = concatenatedGuids.Trim();
            string[] guidsAsStringList = concatenatedGuids.Split(',');
            List<Guid> guidList = new List<Guid>();
            foreach (string guidAsText in guidsAsStringList)
            {
                if (!String.IsNullOrWhiteSpace(guidAsText))
                {
                    Guid id;
                    if (!Guid.TryParse(guidAsText.Trim(), out id))
                    {
                        string msg = "Invalid value (" + guidAsText + ") within the '" + 
                            paramDescription + "' input: " + guidAsText;

                        throw new Pic4PicArgumentException(msg, paramName);
                    }

                    if (id == Guid.Empty)
                    {
                        string msg = "Empty '" + paramDescription + "' within the input : " + guidAsText;
                        throw new Pic4PicArgumentException(msg, paramName);
                    }

                    guidList.Add(id);
                }
            }

            if (guidList.Count == 0)
            {
                string msg = paramDescription + " could not be derived from the input: " + concatenatedGuids;
                throw new Pic4PicArgumentException(msg, paramName);
            }

            if (guidList.Count > Byte.MaxValue)
            {
                throw new Pic4PicArgumentException("Too many menu category IDs: " + guidList.Count.ToString(), paramName);
            }

            return guidList;
        }
    }
}
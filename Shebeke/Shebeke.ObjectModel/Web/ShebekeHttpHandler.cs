using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Diagnostics;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public abstract class ShebekeHttpHandler : IHttpHandler
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
            catch (ShebekeArgumentException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeHttpHandler.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());                
                
                Trace.WriteLine(errorLog, LogCategory.Error);

                context.Response.Write(ex.Message);
            }
            catch (ShebekeException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeHttpHandler.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                context.Response.Write(ex.Message);

                if (ex is ShebekeAuthException)
                {
                    context.Response.Write("Erişim hakkın yok, veya ona benzer bişey!");
                }
            }
            catch (SqlException ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeHttpHandler.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                context.Response.Write("Beklenmeyen veritabanı hatası! Oluyor bazen!");
            }
            catch (Exception ex)
            {
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeHttpHandler.cs",
                            methodName,
                            "ProcessWebRequest",
                            user == null ? 0 : user.UserId,
                            user == null ? 0 : user.SplitId,
                            ex.ToString());

                Trace.WriteLine(errorLog, LogCategory.Error);

                context.Response.Write("Beklenmeyen bir hata oluştu! Sorun bizde");
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

                if(theUser.UserId > 0)
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
                throw new ShebekeArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
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
                throw new ShebekeArgumentException(paramDescription + " hasn't been provided", paramName);
            }

            Guid guidValue = Guid.Empty;
            guidValueAsStr = guidValueAsStr.Trim();
            if (!Guid.TryParse(guidValueAsStr, out guidValue))
            {
                throw new ShebekeArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
            }

            if (guidValue == Guid.Empty)
            {
                throw new ShebekeArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
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
                throw new ShebekeArgumentException(paramDescription + " is too long", paramName);
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
                throw new ShebekeArgumentException(paramDescription + " cannot be empty", paramName);
            }

            name = name.Trim();
            if (name.Length > maxLength)
            {
                throw new ShebekeArgumentException(paramDescription + " is too long", paramName);
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
                throw new ShebekeArgumentException(paramDescription + " cannot be empty", paramName);
            }

            integerValueAsString = integerValueAsString.Trim();
            
            int integerValue;
            if (!Int32.TryParse(integerValueAsString, out integerValue))
            {
                throw new ShebekeArgumentException(paramDescription + " is not an integer", paramName);
            }

            if (integerValue < min || integerValue > max)
            {
                throw new ShebekeArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
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
                throw new ShebekeArgumentException(paramDescription + " cannot be empty", paramName);
            }

            numberAsString = numberAsString.Trim();

            long number;
            if (!Int64.TryParse(numberAsString, out number))
            {
                throw new ShebekeArgumentException(paramDescription + " is not an Int64", paramName);
            }

            if (number < min || number > max)
            {
                throw new ShebekeArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
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
                throw new ShebekeArgumentException(paramDescription + " cannot be empty", paramName);
            }

            dateTimeValueAsString = dateTimeValueAsString.Trim();

            DateTime dateTimeValue;
            if (!DateTime.TryParse(dateTimeValueAsString, out dateTimeValue))
            {
                throw new ShebekeArgumentException(paramDescription + " is not a date-time", paramName);
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
                throw new ShebekeArgumentException("Invalid Action Type: " + action, "act");
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
                throw new ShebekeArgumentException(msg, paramName);
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

                        throw new ShebekeArgumentException(msg, paramName);
                    }

                    if (id == Guid.Empty)
                    {
                        string msg = "Empty '" + paramDescription + "' within the input : " + guidAsText;
                        throw new ShebekeArgumentException(msg, paramName);
                    }

                    guidList.Add(id);
                }
            }

            if (guidList.Count == 0)
            {
                string msg = paramDescription + " could not be derived from the input: " + concatenatedGuids;
                throw new ShebekeArgumentException(msg, paramName);
            }

            if (guidList.Count > Byte.MaxValue)
            {
                throw new ShebekeArgumentException("Too many menu category IDs: " + guidList.Count.ToString(), paramName);
            }

            return guidList;
        }
    }
}
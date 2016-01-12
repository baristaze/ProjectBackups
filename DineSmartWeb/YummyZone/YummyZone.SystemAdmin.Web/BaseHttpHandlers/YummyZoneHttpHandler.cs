using System;
using System.Web;
using System.Data.SqlClient;
using System.Collections.Generic;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    public abstract class YummyZoneHttpHandler : IHttpHandler
    {
        protected enum Source
        {
            Url,
            Form
        }

        public virtual bool IsReusable { get { return false; } }
        
        public virtual void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoServerCaching();
            context.Response.Expires = -10000;

            try
            {
                object o = ProcessWebRequest(context);
                OnSuccess(context, o);
            }
            catch (YummyZoneArgumentException ex)
            {
                context.Response.Write(ex.Message);
            }
            catch (YummyZoneException ex)
            {
                context.Response.Write(ex.Message);
            }
            catch (SqlException)
            {
                context.Response.Write("Database operation failed");
            }
            catch (Exception)
            {
                context.Response.Write("Unknown exception occurred.");
            }

            context.Response.End();
        }

        protected abstract object ProcessWebRequest(HttpContext context);

        protected virtual void OnSuccess(HttpContext context, object result) 
        {
            context.Response.Write(result.ToString());
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
                throw new YummyZoneArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
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
                throw new YummyZoneArgumentException(paramDescription + " hasn't been provided", paramName);
            }

            Guid guidValue = Guid.Empty;
            guidValueAsStr = guidValueAsStr.Trim();
            if (!Guid.TryParse(guidValueAsStr, out guidValue))
            {
                throw new YummyZoneArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
            }

            if (guidValue == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Invalid " + paramDescription + ": " + guidValueAsStr, paramName);
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
                throw new YummyZoneArgumentException(paramDescription + " is too long", paramName);
            }

            return Helpers.UppercaseFirst(name);
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
                throw new YummyZoneArgumentException(paramDescription + " cannot be empty", paramName);
            }

            name = name.Trim();
            if (name.Length > maxLength)
            {
                throw new YummyZoneArgumentException(paramDescription + " is too long", paramName);
            }

            if (upperCaseFirstChar)
            {
                name = Helpers.UppercaseFirst(name);
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
                throw new YummyZoneArgumentException(paramDescription + " cannot be empty", paramName);
            }

            integerValueAsString = integerValueAsString.Trim();
            
            int integerValue;
            if (!Int32.TryParse(integerValueAsString, out integerValue))
            {
                throw new YummyZoneArgumentException(paramDescription + " is not an integer", paramName);
            }

            if (integerValue < min || integerValue > max)
            {
                throw new YummyZoneArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
            }

            return integerValue;
        }

        protected decimal GetMandatoryDecimal(HttpContext context, string paramName, string paramDescription, decimal min, decimal max, Source source)
        {
            string decimalValueAsString = null;
            if (source == Source.Url)
            {
                decimalValueAsString = context.Request.Params[paramName];
            }
            else
            {
                decimalValueAsString = context.Request.Form[paramName];
            }

            if (String.IsNullOrWhiteSpace(decimalValueAsString))
            {
                throw new YummyZoneArgumentException(paramDescription + " cannot be empty", paramName);
            }

            decimalValueAsString = decimalValueAsString.Trim();

            decimal decimalValue;
            if (!Decimal.TryParse(decimalValueAsString, out decimalValue))
            {
                throw new YummyZoneArgumentException(paramDescription + " is not a decimal", paramName);
            }

            if (decimalValue < min || decimalValue > max)
            {
                throw new YummyZoneArgumentException(paramDescription + " must be between " + min + " and " + max, paramName);
            }

            return decimalValue;
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
                throw new YummyZoneArgumentException(paramDescription + " cannot be empty", paramName);
            }

            dateTimeValueAsString = dateTimeValueAsString.Trim();

            DateTime dateTimeValue;
            if (!DateTime.TryParse(dateTimeValueAsString, out dateTimeValue))
            {
                throw new YummyZoneArgumentException(paramDescription + " is not a date-time", paramName);
            }

            return dateTimeValue;
        }

        protected List<Guid> GetMandatoryGuidList(HttpContext context, string paramName, string paramDescription)
        {
            string concatenatedGuids = context.Request.Params[paramName];
            if (String.IsNullOrWhiteSpace(concatenatedGuids))
            {
                string msg = "There hasn't been specified any " + paramDescription + " as an input";
                throw new YummyZoneArgumentException(msg, paramName);
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

                        throw new YummyZoneArgumentException(msg, paramName);
                    }

                    if (id == Guid.Empty)
                    {
                        string msg = "Empty '" + paramDescription + "' within the input : " + guidAsText;
                        throw new YummyZoneArgumentException(msg, paramName);
                    }

                    guidList.Add(id);
                }
            }

            if (guidList.Count == 0)
            {
                string msg = paramDescription + " could not be derived from the input: " + concatenatedGuids;
                throw new YummyZoneArgumentException(msg, paramName);
            }

            if (guidList.Count > Byte.MaxValue)
            {
                throw new YummyZoneArgumentException("Too many menu category IDs: " + guidList.Count.ToString(), paramName);
            }

            return guidList;
        }
    }
}
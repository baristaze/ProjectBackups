using System;
using System.Diagnostics;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public abstract class ConfigBase
    {
        public abstract bool Init();
        protected abstract string GetFromStore(string key);

        public string Get(string key)
        {
            return this.Get(key, true, null);
        }

        public string Get(string key, string defaultVal)
        {
            return this.Get(key, false, defaultVal);
        }

        protected string Get(string key, bool throwOnError, string defaultVal)
        {
            // Trace.WriteLine("Getting " + key, LogCategory.Info);

            string val = null;

            try
            {
                val = this.GetFromStore(key);
            }
            catch
            {
                if (throwOnError)
                {
                    throw;
                }
            }

            if (String.IsNullOrWhiteSpace(val))
            {
                string msg = "Coudn't retrieve '" + key + "' from settings";
                if (throwOnError)
                {
                    string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ConfigBase.cs",
                            "Get",
                            "GetFromStore",
                            0,
                            0,
                            msg);

                    Trace.WriteLine(errorLog, LogCategory.Error);

                    throw new CrossplException(msg);
                }
                else
                {
                    msg += ". Using default value";
                    Trace.WriteLine(msg, LogCategory.Info);
                    return defaultVal;
                }
            }
            else
            {
                // Trace.WriteLine(key + " is retrieved successfully: " + val.Trim(), LogCategory.Info);
                return val.Trim();
            }
        }

        public int GetAsInt(string key, int min, int max)
        {
            return this.GetAsInt(key, true, 0, min, max);
        }

        public int GetAsInt(string key, int defaultVal, int min, int max)
        {
            return this.GetAsInt(key, false, defaultVal, min, max);
        }

        protected int GetAsInt(string key, bool throwOnError, int defaultVal, int min, int max)
        {
            int val = defaultVal;
            string valTxt = this.Get(key, throwOnError, defaultVal.ToString());
            if (!Int32.TryParse(valTxt, out val))
            {
                string msg = "Type of '" + key + "' is not an integer";
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ConfigBase.cs",
                            "GetAsInt",
                            "TryParse",
                            0,
                            0,
                            msg);
                
                Trace.WriteLine(errorLog, LogCategory.Error);

                if (throwOnError)
                {
                    throw new CrossplException(msg);
                }
                else
                {
                    msg += ". Using default value";
                    Trace.WriteLine(msg, LogCategory.Info);
                    val = defaultVal;
                }
            }

            if (val < min)
            {
                throw new CrossplException(key + " may not be smaller than " + min.ToString());
            }

            if (val > max)
            {
                throw new CrossplException(key + " may not be bigger than " + max.ToString());
            }

            return val;
        }

        public bool GetAsBool(string key)
        {
            return this.GetAsBool(key, true, false);
        }

        public bool GetAsBool(string key, bool defaultVal)
        {
            return this.GetAsBool(key, false, defaultVal);
        }

        protected bool GetAsBool(string key, bool throwOnError, bool defaultVal)
        {
            bool val = defaultVal;
            string valTxt = this.Get(key, throwOnError, defaultVal.ToString());
            if (!TryParseAsBool(valTxt, out val))
            {
                string msg = "Type of '" + key + "' is not a boolean";
                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ConfigBase.cs",
                            "GetAsBool",
                            "TryParseAsBool",
                            0,
                            0,
                            msg);

                Trace.WriteLine(errorLog, LogCategory.Error);

                if (throwOnError)
                {
                    throw new CrossplException(msg);
                }
                else
                {
                    msg += ". Using default value";
                    Trace.WriteLine(msg, LogCategory.Info);
                    return defaultVal;
                }
            }
            return val;
        }

        protected bool TryParseAsBool(string text, out bool result)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                result = false;
                return false;
            }

            text = text.Trim().ToLowerInvariant();
            if (text == "0" || text == "false" || text == "no")
            {
                result = false;
                return true;
            }
            else if (text == "1" || text == "true" || text == "yes")
            {
                result = true;
                return true;
            }

            result = false;
            return false;
        }
    }
}


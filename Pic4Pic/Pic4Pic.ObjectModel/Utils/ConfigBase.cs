using System;
using System.Diagnostics;
using System.Globalization;

namespace Pic4Pic.ObjectModel
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
                if (throwOnError)
                {
                    Logger.bag()
                        .Add("action", "get config value from store")
                        .Add("key", key)
                        .LogError();

                    throw new Pic4PicException("Coudn't retrieve the key value from settings");
                }
                else
                {
                    Logger.bag()
                        .Add("action", "get config value from store")
                        .Add("key", key)
                        .LogInfo("Coudn't retrieve the key value from settings. Using default value.");

                    return defaultVal;
                }
            }
            else
            {
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
                            "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ConfigBase.cs",
                            "GetAsInt",
                            "TryParse",
                            Guid.Empty,
                            0,
                            msg);

                if (throwOnError)
                {
                    Logger.bag()
                    .Add("action", "get setting value as int")
                    .Add("key", key)
                    .LogError("Type of '" + key + "' is not an integer");

                    throw new Pic4PicException(msg);
                }
                else
                {
                    Logger.bag()
                    .Add("action", "get setting value as int")
                    .Add("key", key)
                    .LogInfo("Type of '" + key + "' is not an integer. Using default value.");

                    val = defaultVal;
                }
            }

            if (val < min)
            {
                throw new Pic4PicException(key + " may not be smaller than " + min.ToString());
            }

            if (val > max)
            {
                throw new Pic4PicException(key + " may not be bigger than " + max.ToString());
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
                if (throwOnError)
                {
                    Logger.bag()
                    .Add("action", "get setting value as int")
                    .Add("key", key)
                    .LogError("Type of '" + key + "' is not a boolean.");

                    throw new Pic4PicException("Type of '" + key + "' is not a boolean.");
                }
                else
                {
                    Logger.bag()
                    .Add("action", "get setting value as int")
                    .Add("key", key)
                    .LogInfo("Type of '" + key + "' is not a boolean. Using default value.");

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

        protected MailSettings GetMailSettings()
        {
            MailSettings settings = new MailSettings();
            this.ReadMailSettings(settings);
            return settings;
        }

        protected void ReadMailSettings(MailSettings settings)
        {
            settings.IsEmailEnabled = this.GetAsBool("EMail_Enabled", true);
            settings.SmtpHost = this.Get("EMail_SmtpHost", "smtp.gmail.com");
            settings.SmtpPort = this.GetAsInt("EMail_SmtpPort", 587, 80, (int)short.MaxValue);
            settings.SenderName = this.Get("EMail_SenderName", "Smart Diner");
            settings.SenderEmail = this.Get("EMail_SenderEmail", "appsicle@gmail.com");
            settings.SenderPswd = this.Get("EMail_SenderPswd");
            settings.UseSSL = this.GetAsBool("EMail_UseSSL", true);
        }

        public MailSettingsEx GetMailSettingsEx()
        {
            MailSettingsEx settings = new MailSettingsEx();
            this.ReadMailSettings(settings);

            settings.EmailToList = this.Get("EMail_ToList", String.Empty);
            settings.EmailCCList = this.Get("EMail_CCList", String.Empty);
            settings.EmailBCCList = this.Get("EMail_BCCList", String.Empty);

            return settings;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class LogProperty : IVerifiable
    {
        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public string Value { get; set; }

        public LogProperty()
        {
        }

        public LogProperty(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new Pic4PicArgumentException("Name of LogProperty may not be empty", "Name");
            }
        }

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(this.Name))
            {
                return String.Empty;
            }

            if (String.IsNullOrWhiteSpace(this.Value))
            {
                return Name;
            }

            return this.Name + "=" + this.Value;
        }
    }

    [DataContract()]
    public class LogBag : IVerifiable
    {
        private const int Debug = 1;
	    private const int Verbose = 2;
	    private const int Info = 3;
	    private const int Metric = 4;
	    private const int Warning = 5;
	    private const int Error = 6;

        public const String TagAppType = "AppType";
        public const String TagFileName = "File";
        public const String TagClassName = "Class";
        public const String TagMethodName = "Method";
        public const String TagLineNumber = "Line";
        public const String TagLogId = "LogId";
        public const String TagLogTime = "LogTimeUTC";
        private const String TagMessage = "Message";
        private const String TagException = "Exception";
        private const String TagObjectType = "ObjectType";
        private const String TagObjectId = "ObjectId";
        private const String TagParam = "Param";
        private const String TagLevel = "Level";        

        [DataMember()]
        public List<LogProperty> Pairs
        {
            get
            {
                return this.pairs;
            }
            set
            {
                if (value != null)
                {
                    this.pairs = value;
                }
                else
                {
                    this.pairs = new List<LogProperty>();
                }
            }
        }

        private List<LogProperty> pairs = new List<LogProperty>();

        public void Validate()
        {
            if (this.pairs.Count <= 0)
            {
                throw new Pic4PicArgumentException("Property bags in the LogBag may not be empty", "Pairs");
            }

            foreach (LogProperty pair in this.pairs)
            {
                pair.Validate();
            }
        }

        public String GetErrorLevel()
        {
            foreach (LogProperty pair in this.pairs)
            {
                if (pair.Name.ToLower().Equals(TagLevel.ToLower()))
                {
                    return this.convertClientErrorLevelTo(pair.Value);
                }
            }

            return LogCategory.Verbose;
        }

        public String GetByTag(String tagName)
        {
            foreach (LogProperty pair in this.pairs)
            {
                if (pair.Name.ToLower().Equals(tagName.ToLower()))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public Guid GetGuidByTag(String tagName)
        {
            String val = this.GetByTag(tagName);
            if (String.IsNullOrWhiteSpace(val))
            {
                return Guid.Empty;
            }

            Guid guidVal = Guid.Empty;
            Guid.TryParse(val, out guidVal);
            return guidVal;
        }

        public int GetIntByTag(String tagName, int defaultVal)
        {
            String val = this.GetByTag(tagName);
            if (String.IsNullOrWhiteSpace(val))
            {
                return defaultVal;
            }

            Int32 intVal = defaultVal;
            Int32.TryParse(val, out intVal);
            return intVal;
        }

        public DateTime GetDateTimeByTag(String tagName)
        {
            String val = this.GetByTag(tagName);
            if (String.IsNullOrWhiteSpace(val))
            {
                return default(DateTime);
            }

            DateTime dateTimeVal = default(DateTime);
            try
            {
                dateTimeVal = DateTime.ParseExact(val, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError("Parsing date time has failed.");
            }
            
            return dateTimeVal;
        }

        private String convertClientErrorLevelTo(String clientErrorLevel)
        {
            switch (clientErrorLevel.ToLower())
            {
                case "error":
                    return LogCategory.Error;

                case "warning":
                    return LogCategory.Warning;

                case "metric":
                    return LogCategory.Metric;

                case "info":
                    return LogCategory.Info;

                case "debug":
                    return LogCategory.Debug;

                default:
                    return LogCategory.Verbose;
            }
        }

        public override string ToString()
        {
            if (this.pairs.Count <= 0)
            {
                return String.Empty;
            }

            bool appended = false;
            StringBuilder builder = new StringBuilder();

            LogProperty exceptionLogBagItem = null;
            for (int x = this.pairs.Count - 1; x >= 0; x--)
            {
                LogProperty logBagItem = this.pairs[x];
                if (logBagItem.Name.Equals(TagException))
                {
                    // this will be inserted at the end
                    exceptionLogBagItem = logBagItem;
                }
                else
                {
                    if (appended)
                    {
                        builder.Append("[;]");
                    }

                    builder.Append("[" + logBagItem.ToString() + "]");
                    appended = true;
                }
            }

            if (exceptionLogBagItem != null)
            {
                if (appended)
                {
                    builder.Append("[;]");
                }

                builder.Append("[" + exceptionLogBagItem.ToString() + "]");
                appended = true;
            }

            return builder.ToString();
        }

        public LogBag Add(String tag, String value)
        {

            if (!String.IsNullOrWhiteSpace(tag))
            {
                this.pairs.Add(new LogProperty(tag, value));
            }

            return this;
        }

        public LogBag Add(Exception ex)
        {

            if (ex != null)
            {
                return this.Add(TagException, ex.ToString());
            }

            return this;
        }

        public LogBag AddObject(String objectType, Guid objectId)
        {
            this.Add(TagObjectId, objectId.ToString());
            return this.Add(TagObjectType, objectType);
        }

        public LogBag AddParam(Object obj)
        {
            return this.Add(TagParam, (obj == null ? null : obj.ToString()));
        }

        public void LogError()
        {
            this.LogError(null);
        }

        public void LogError(String message)
        {
            this.log(Error, message);
        }

        public void LogWarning()
        {
            this.LogWarning(null);
        }

        public void LogWarning(String message)
        {
            this.log(Warning, message);
        }

        public void LogMetric()
        {
            this.LogMetric(null);
        }

        public void LogMetric(String message)
        {
            this.log(Metric, message);
        }

        public void LogInfo()
        {
            this.LogInfo(null);
        }

        public void LogInfo(String message)
        {
            this.log(Info, message);
        }

        public void LogVerbose()
        {
            this.LogVerbose(null);
        }

        public void LogVerbose(String message)
        {
            this.log(Verbose, message);
        }

        public void LogDebug()
        {
            this.LogDebug(null);
        }

        public void LogDebug(String message)
        {
            this.log(Verbose, message);
        }

        protected String getLevelAsText(int level)
        {
            switch (level)
            {
                case Error:
                    return "Error";

                case Warning:
                    return "Warning";

                case Metric:
                    return "Metric";

                case Info:
                    return "Info";

                case Debug:
                    return "Debug";

                default:
                    return "Verbose";
            }
        }
        
        protected void log(int level, String message)
        {

            // add level
            this.Add(TagLevel, this.getLevelAsText(level));

            // add message
            if (!String.IsNullOrWhiteSpace(message))
            {
                this.Add(TagMessage, message);
            }

            String log = this.ToString();
            System.Diagnostics.Trace.WriteLine(log, "P4P_" + this.getLevelAsText(level));
        }

        public static LogBag Parse(String logLine)
        {
            if (String.IsNullOrWhiteSpace(logLine)) 
            {
                return null;
            }

            if (!logLine.StartsWith("P4P_"))
            {
                return null;
            }

            logLine = logLine.Substring("P4P_".Length);
            int index = logLine.IndexOf(":");
            if(index <= 0)
            {
                return null;
            }
            
            string logType = logLine.Substring(0, index).Trim();
            if (logType.Length == 0)
            {
                return null;
            }
            
            logLine = logLine.Substring(index).TrimStart(' ', ':');
            
            // split
            String[] tokens = logLine.Split(new string[] {"[;]"}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return null;
            }

            Dictionary<String, String> pairs = new Dictionary<string, string>();
            pairs.Add(LogBag.TagLevel, logType);

            foreach (String token in tokens)
            {
                if (token.Trim().Length > 0)
                {
                    String temp = token.Trim();
                    index = temp.IndexOf("=");
                    if (index > 0 && index < (temp.Length-1))
                    {
                        String key = temp.Substring(0, index).Trim(' ', '[', ']');
                        String val = temp.Substring(index + 1).Trim(' ', '[', ']');

                        if (key.Length > 0 && val.Length > 0)
                        {
                            if (pairs.ContainsKey(key))
                            {
                                pairs[key] = val;
                            }
                            else
                            {
                                pairs.Add(key, val);
                            }
                        }
                    }
                }
            }

            if (pairs.Count <= 1)
            {
                return null;
            }

            LogBag bag = new LogBag();
            foreach (String key in pairs.Keys)
            {
                bag.Pairs.Add(new LogProperty(key, pairs[key]));
            }

            return bag;
        }
    }
}

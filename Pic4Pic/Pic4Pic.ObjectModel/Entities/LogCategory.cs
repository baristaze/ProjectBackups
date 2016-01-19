namespace Pic4Pic.ObjectModel
{
    public class LogCategory
    {
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Metric = "Metric";
        public const string Info = "Info";
        public const string Verbose = "Verbose";
        public const string Debug = "Debug";
    }

    public enum LogReportingLevel
    {
        None = 0,
        OnlyErrors = 1,
        WarningsAndAbove = 2,
        MetricsAndAbove=3,
        InfoAndAbove = 4,
        VerboseAndAbove = 5,
        All = 6,
    }
}
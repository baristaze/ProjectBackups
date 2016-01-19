using System;

namespace Crosspl.ObjectModel
{
    public abstract class LogParser<T> where T : IPrintable
    {
        public const char PropertySeparator = ';';
        public const char PairSeparator = '=';
        public const char PropertyStart = '[';
        public const char PropertyEnd = ']';

        public string ParseNextAndValidateName(string message, out NameValuePair<string> pair, string name)
        {
            pair = null;
            message = ParseNext(message, out pair);
            if (pair == null)
            {
                throw new CrossplException("Misformed log entry around " + name);
            }

            if (String.Compare(pair.Name, name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new CrossplException("Missing Tag in Log: " + name);
            }

            return message;
        }

        public string ParseNextAndValidateName(string message, out NameValuePair<int> pair, string name, int min, int max)
        {
            pair = null;
            message = ParseNext(message, out pair, min, max);
            if (pair == null)
            {
                throw new CrossplException("Misformed log entry around " + name);
            }

            if (String.Compare(pair.Name, name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new CrossplException("Missing Tag in Log: " + name);
            }

            return message;
        }

        public string ParseNextAndValidateName(string message, out NameValuePair<long> pair, string name, long min, long max)
        {
            pair = null;
            message = ParseNext(message, out pair, min, max);
            if (pair == null)
            {
                throw new CrossplException("Misformed log entry around " + name);
            }

            if (String.Compare(pair.Name, name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new CrossplException("Missing Tag in Log: " + name);
            }

            return message;
        }

        public string ParseNext(string message, out NameValuePair<string> pair)
        {
            pair = null;

            if (String.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            string pairText = message;
            string remainingText = null;
            int index = message.IndexOf(PropertySeparator);
            if (index > 0)
            {
                pairText = message.Substring(0, index);

                if (index < message.Length - 1)
                {
                    remainingText = message.Substring(index + 1);
                }
            }

            pair = ParsePair(pairText);
            return remainingText;
        }

        public string ParseNext(string message, out NameValuePair<int> pair, int min, int max)
        {
            pair = null;

            if (String.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            string pairText = message;
            string remainingText = null;
            int index = message.IndexOf(PropertySeparator);
            if (index > 0)
            {
                pairText = message.Substring(0, index);

                if (index < message.Length - 1)
                {
                    remainingText = message.Substring(index + 1);
                }
            }

            pair = ParsePairAsInt(pairText, min, max);
            return remainingText;
        }

        public string ParseNext(string message, out NameValuePair<long> pair, long min, long max)
        {
            pair = null;

            if (String.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            string pairText = message;
            string remainingText = null;
            int index = message.IndexOf(PropertySeparator);
            if (index > 0)
            {
                pairText = message.Substring(0, index);

                if (index < message.Length - 1)
                {
                    remainingText = message.Substring(index + 1);
                }
            }

            pair = ParsePairAsLong(pairText, min, max);
            return remainingText;
        }

        public NameValuePair<string> ParsePair(string pairText)
        {
            if (String.IsNullOrWhiteSpace(pairText))
            {
                return null;
            }

            pairText = pairText.TrimStart(PropertyStart);
            pairText = pairText.TrimEnd(PropertyEnd);

            int index = pairText.IndexOf(PairSeparator);
            if (index <= 0 || index == pairText.Length - 1)
            {
                return null;
            }

            string name = pairText.Substring(0, index);
            string value = pairText.Substring(index + 1);

            return new NameValuePair<string>(name, value);
        }

        public NameValuePair<int> ParsePairAsInt(string pairText, int min, int max)
        {
            NameValuePair<string> pair = ParsePair(pairText);
            if (pair == null)
            {
                return null;
            }

            int val = 0;
            if (!Int32.TryParse(pair.Value, out val))
            {
                return null;
            }

            if (val < min || val > max)
            {
                return null;
            }

            return new NameValuePair<int>(pair.Name, val);
        }

        public NameValuePair<long> ParsePairAsLong(string pairText, long min, long max)
        {
            NameValuePair<string> pair = ParsePair(pairText);
            if (pair == null)
            {
                return null;
            }

            long val = 0;
            if (!Int64.TryParse(pair.Value, out val))
            {
                return null;
            }

            if (val < min || val > max)
            {
                return null;
            }

            return new NameValuePair<long>(pair.Name, val);
        }

        public string ConsumePreamble(string message, string preamble)
        {
            if (message.StartsWith(preamble))
            {
                return message.Substring(preamble.Length);
            }

            throw new CrossplException("Log message doesn't have " + preamble + " preamble.");
        }

        public abstract T Parse(WADLogsTable log);
    }
}

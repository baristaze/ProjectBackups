using System;
using System.Globalization;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    public class Helpers
    {
        private const int minItemCountForTopRate2 = 5;
        private const int minItemCountForTopRate3 = 8;
        private const decimal minimumRateToRecommend = (decimal)2.51;

        private const int maxCheckinInLast24Hours = 8;
        private const int maxCheckinPerVenueInLast24Hours = 3;
        private const int maxMinutesPerCheckinSession = 150;
        private const int maxMessageForDiner = 50;
        private const int maxCouponForDiner = 50;
        private const int maxMenuItemRateCount = 8;
        private const int maxLengthOfAnswerForOpenEndedQuestion = 500;

        private const int favMaxMenuItem = 50;
        private const int favMaxVenue = 50;
        private const int historyMaxElements = 150;

        private const int showMilesInsteadOfYardsLimit = 300;

        private const decimal favMinMenuItemRate = (decimal)3.5;
        private const decimal favMinVenueRate = (decimal)3.5;

        private static readonly TimeZoneInfo DefaultTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            }
        }

        public static string AbsPathPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["AbsPathPrefix"];
            }
        }

        public static string ImageFileUrlTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageFileUrlTemplate"];
            }
        }

        public static string LogoFileUrlTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings["LogoFileUrlTemplate"];
            }
        }
        
        public static bool ConvertToBoolTrue(string valAsText)
        {
            bool val = false;            
            if (valAsText != null)
            {
                valAsText = valAsText.Trim();
                valAsText.ToLower();
                if (valAsText == "1" || valAsText == "true" || valAsText == "yes" || valAsText == "on")
                {
                    val = true;
                }
            }

            return val;
        }

        public static bool ConvertToBoolFalse(string valAsText)
        {
            bool val = false;
            if (valAsText != null)
            {
                valAsText = valAsText.Trim();
                valAsText.ToLower();
                if (valAsText == "0" || valAsText == "false" || valAsText == "no" || valAsText == "off")
                {
                    val = true;
                }
            }

            return val;
        }

        public static bool ConvertibleToBool(string valAsText)
        {
            return ConvertToBoolTrue(valAsText) || ConvertToBoolFalse(valAsText);
        }

        public static bool UseClientGuid
        {
            get
            {
                string valAsText = ConfigurationManager.AppSettings["UseClientGuid"];
                return ConvertToBoolTrue(valAsText);
            }
        }

        public static int MinUserNameLength
        {
            get
            {
                int min = Identity.MinUserNameLength;
                string minAsText = ConfigurationManager.AppSettings["MinUserNameLength"];
                Int32.TryParse(minAsText, out min);
                return min;
            }
        }

        public static int MinPasswordLength
        {
            get
            {
                int min = Identity.MinPasswordLength;
                string minAsText = ConfigurationManager.AppSettings["MinPasswordLength"];
                Int32.TryParse(minAsText, out min);
                return min;
            }
        }

        public static UsernameType UserNameFormat
        {
            get
            {
                UsernameType userNameFormat = Identity.DefaultUserNameFormat;

                string cfg = ConfigurationManager.AppSettings["UserNameFormat"];
                if (!String.IsNullOrWhiteSpace(cfg))
                {
                    cfg = cfg.Trim();

                    if (String.Compare(cfg, "FreeText", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        userNameFormat = UsernameType.FreeText;
                    }
                    else if (String.Compare(cfg, "Email", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        userNameFormat = UsernameType.Email;
                    }
                    else if (String.Compare(cfg, "Guid", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        userNameFormat = UsernameType.Guid;
                    }
                    else if (String.Compare(cfg, "EmailOrGuid", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        userNameFormat = UsernameType.EmailOrGuid;
                    }
                }

                return userNameFormat;
            }
        }

        public static int MinItemCountForTopRate2
        {
            get
            {
                int min = minItemCountForTopRate2;
                string minAsText = ConfigurationManager.AppSettings["MinItemCountForTopRate2"];
                Int32.TryParse(minAsText, out min);
                return min;
            }
        }

        public static int MinItemCountForTopRate3
        {
            get
            {
                int min = minItemCountForTopRate3;
                string minAsText = ConfigurationManager.AppSettings["MinItemCountForTopRate3"];
                Int32.TryParse(minAsText, out min);
                return min;
            }
        }

        public static decimal MinimumRateToRecommend
        {
            get
            {
                decimal minRate = minimumRateToRecommend;
                string minRateAsText = ConfigurationManager.AppSettings["MinimumRateToRecommend"];
                Decimal.TryParse(minRateAsText, out minRate);
                return minRate;
            }
        }

        public static double Default_SearchVenue_LatitudeThreshold
        {
            get
            {
                double threshold = OM.Venue.Default_SearchVenue_LatitudeThreshold;
                string thresholdAsText = ConfigurationManager.AppSettings["SearchVenue_LatitudeThreshold"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_SearchVenue_LongitudeThreshold
        {
            get
            {
                double threshold = OM.Venue.Default_SearchVenue_LongitudeThreshold;
                string thresholdAsText = ConfigurationManager.AppSettings["SearchVenue_LongitudeThreshold"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_SearchVenue_RangeLimitInMiles
        {
            get
            {
                double threshold = OM.Venue.Default_SearchVenue_RangeLimitInMiles;
                string thresholdAsText = ConfigurationManager.AppSettings["SearchVenue_RangeLimitInMiles"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_RedeemCoupon_LatitudeThreshold
        {
            get
            {
                double threshold = OM.Venue.Default_RedeemCoupon_LatitudeThreshold;
                string thresholdAsText = ConfigurationManager.AppSettings["RedeemCoupon_LatitudeThreshold"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_RedeemCoupon_LongitudeThreshold
        {
            get
            {
                double threshold = OM.Venue.Default_RedeemCoupon_LongitudeThreshold;
                string thresholdAsText = ConfigurationManager.AppSettings["RedeemCoupon_LongitudeThreshold"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_RedeemCoupon_RangeLimitInMiles
        {
            get
            {
                double threshold = OM.Venue.Default_RedeemCoupon_RangeLimitInMiles;
                string thresholdAsText = ConfigurationManager.AppSettings["RedeemCoupon_RangeLimitInMiles"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static double Default_SendFeedback_RangeLimitInMiles
        {
            get
            {
                double threshold = OM.Venue.Default_SendFeedback_RangeLimitInMiles;
                string thresholdAsText = ConfigurationManager.AppSettings["SendFeedback_RangeLimitInMiles"];
                Double.TryParse(thresholdAsText, out threshold);
                return threshold;
            }
        }

        public static int MaxNearbyVenues
        {
            get
            {
                int max = OM.Venue.DefaultMaxNearbyVenues;
                string maxAsText = ConfigurationManager.AppSettings["MaxNearbyVenues"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxCheckinInLast24Hours
        {
            get
            {
                int max = maxCheckinInLast24Hours;
                string maxAsText = ConfigurationManager.AppSettings["MaxCheckinInLast24Hours"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxCheckinPerVenueInLast24Hours
        {
            get
            {
                int max = maxCheckinPerVenueInLast24Hours;
                string maxAsText = ConfigurationManager.AppSettings["MaxCheckinPerVenueInLast24Hours"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxMinutesPerCheckinSession
        {
            get
            {
                int max = maxMinutesPerCheckinSession;
                string maxAsText = ConfigurationManager.AppSettings["MaxMinutesPerCheckinSession"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxMessageForDiner
        {
            get
            {
                int max = maxMessageForDiner;
                string maxAsText = ConfigurationManager.AppSettings["MaxMessageForDiner"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxCouponForDiner
        {
            get
            {
                int max = maxCouponForDiner;
                string maxAsText = ConfigurationManager.AppSettings["MaxCouponForDiner"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxMenuItemRateCount
        {
            get
            {
                int max = maxMenuItemRateCount;
                string maxAsText = ConfigurationManager.AppSettings["MaxMenuItemRateCount"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int MaxLengthOfAnswerForOpenEndedQuestion
        {
            get
            {
                int max = maxLengthOfAnswerForOpenEndedQuestion;
                string maxAsText = ConfigurationManager.AppSettings["MaxLengthOfAnswerForOpenEndedQuestion"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int FavMaxMenuItem
        {
            get
            {
                int max = favMaxMenuItem;
                string maxAsText = ConfigurationManager.AppSettings["FavMaxMenuItem"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int FavMaxVenue
        {
            get
            {
                int max = favMaxVenue;
                string maxAsText = ConfigurationManager.AppSettings["FavMaxVenue"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int HistoryMaxElements
        {
            get
            {
                int max = historyMaxElements;
                string maxAsText = ConfigurationManager.AppSettings["HistoryMaxElements"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static int ShowMilesInsteadOfYardsLimit
        {
            get
            {
                int max = showMilesInsteadOfYardsLimit;
                string maxAsText = ConfigurationManager.AppSettings["ShowMilesInsteadOfYardsLimit"];
                Int32.TryParse(maxAsText, out max);
                return max;
            }
        }

        public static decimal FavMinMenuItemRate
        {
            get
            {
                decimal min = favMinMenuItemRate;
                string minAsText = ConfigurationManager.AppSettings["FavMinMenuItemRate"];
                Decimal.TryParse(minAsText, out min);
                return min;
            }
        }

         public static decimal FavMinVenueRate
        {
            get
            {
                decimal min = favMinVenueRate;
                string minAsText = ConfigurationManager.AppSettings["FavMinVenueRate"];
                Decimal.TryParse(minAsText, out min);
                return min;
            }
        }

        public static TimeZoneInfo DefaultTimeZone
        {
            get
            {
                int index = -1;
                string indexAsText = ConfigurationManager.AppSettings["DefaultTimeZoneWinIndex"];
                Int32.TryParse(indexAsText, out index);

                TimeZoneInfo tzi = null;
                if (index >= 0)
                {
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                    if (index < timeZones.Count)
                    {
                        tzi = timeZones[index];
                    }
                }

                if (tzi == null)
                {
                    tzi = DefaultTimeZoneInfo;
                }

                return tzi;
            }
        }

        public static string ToDBaseString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public static bool IsValidEmail(string email)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public static Guid InitialMessageGroupId
        {
            get
            {
                Guid id = Guid.Empty;
                string guidAsText = ConfigurationManager.AppSettings["InitialMessageGroupId"];
                Guid.TryParse(guidAsText, out id);
                return id;
            }
        }

        public static Guid InitialMessageChainId
        {
            get
            {
                Guid id = Guid.Empty;
                string guidAsText = ConfigurationManager.AppSettings["InitialMessageChainId"];
                Guid.TryParse(guidAsText, out id);
                return id;
            }
        }

        public static Guid InitialMessageSenderId
        {
            get
            {
                Guid id = Guid.Empty;
                string guidAsText = ConfigurationManager.AppSettings["InitialMessageSenderId"];
                Guid.TryParse(guidAsText, out id);
                return id;
            }
        }

        public static string InitialMessageTitle
        {
            get
            {
                return ConfigurationManager.AppSettings["InitialMessageTitle"];
            }
        }

        public static string InitialMessageContent
        {
            get
            {
                return ConfigurationManager.AppSettings["InitialMessageContent"];
            }
        }

        public static bool ShowPrices
        {
            get
            {
                // default is false
                string boolAsText = ConfigurationManager.AppSettings["ShowPrices"];
                return ConvertToBoolTrue(boolAsText);
            }
        }

        public static bool AllowNullCoordinateOnFeedback
        {
            get
            {
                // default is false
                string boolAsText = ConfigurationManager.AppSettings["AllowNullCoordinateOnFeedback"];
                return ConvertToBoolTrue(boolAsText);
            }
        }
    }
}
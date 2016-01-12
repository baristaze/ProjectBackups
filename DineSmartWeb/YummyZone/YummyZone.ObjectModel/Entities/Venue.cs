using System;
using System.Collections.Generic;
using System.Runtime.Serialization; 

namespace YummyZone.ObjectModel 
{
    public enum VenueStatus
    {
        Draft = 0,
        Active,
        Disabled,
        Removed,
    }

    public partial class Venue : YummyZoneEntity, IEditable
    {
        private const double EarthRadiousInMiles = 3963.1676;
        private const double EarthRadiousInMeters = 6378100.0;
        public const double MeterToYard = 1.0936133;
        public const double MilesToYard = 1760;

        public const double Default_SearchVenue_LatitudeThreshold = 1.0;
        public const double Default_SearchVenue_LongitudeThreshold = 1.0;
        public const double Default_SearchVenue_RangeLimitInMiles = 50.0;

        public const double Default_RedeemCoupon_LatitudeThreshold = 0.02;        
        public const double Default_RedeemCoupon_LongitudeThreshold = 0.02;
        public const double Default_RedeemCoupon_RangeLimitInMiles = 0.3;
        public const double Default_SendFeedback_RangeLimitInMiles = 0.3;
        
        public const int DefaultMaxNearbyVenues = 50;
        
        public Guid ChainId { get; set; }
        public VenueStatus Status { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string MapURL { get; set; }
        public string WebURL { get; set; }
        public byte? TimeZoneWinIndex { get; set; }

        public decimal? LatitudeThresholdForSearch { get; set; }
        public decimal? LongitudeThresholdForSearch { get; set; }
        public decimal? RangeLimitInMilesForSearch { get; set; }
        
        public decimal? LatitudeThresholdForRedeem { get; set; }
        public decimal? LongitudeThresholdForRedeem { get; set; }
        public decimal? RangeLimitInMilesForRedeem { get; set; }
        public decimal? RangeLimitInMilesForFeedback { get; set; }
        
        public Address Address { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        #region Distance

        public static double CalculateDistanceInMiles(double latitude1, double latitude2, double longitude1, double longitude2)
        {
            return Haversine_CalculateDistance(EarthRadiousInMiles, latitude1, latitude2, longitude1, longitude2);
        }

        /*
        public static double CalculateDistanceInMeters(double latitude1, double latitude2, double longitude1, double longitude2)
        {
            return Haversine_CalculateDistance(EarthRadiousInMeters, latitude1, latitude2, longitude1, longitude2);
        }
        */

        private static double Haversine_CalculateDistance(
            double radious,
            double latitude1,
            double latitude2,
            double longitude1,
            double longitude2)
        {
            // see http://en.wikipedia.org/wiki/Haversine_formula

            latitude1 *= (Math.PI / 180.0);
            latitude2 *= (Math.PI / 180.0);
            longitude1 *= (Math.PI / 180.0);
            longitude2 *= (Math.PI / 180.0);

            double deltaLat = latitude2 - latitude1;
            double deltaLong = longitude2 - longitude1;

            double a = Math.Pow(Math.Sin(deltaLat / 2.0), 2.0) +
                   Math.Cos(latitude1) * Math.Cos(latitude2) *
                   Math.Pow(Math.Sin(deltaLong / 2.0), 2.0);

            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return radious * c;
        }

        /*
        private double Vincenty_CalculateDistance(double radious, double latitude1, double latitude2, double longitude1, double longitude2)
        {
            // see http://en.wikipedia.org/wiki/Great-circle_distance

            latitude1 *= (Math.PI / 180.0);
            latitude2 *= (Math.PI / 180.0);
            longitude1 *= (Math.PI / 180.0);
            longitude2 *= (Math.PI / 180.0);

            double deltaLong = longitude2 - longitude1;

            double sinLat1 = Math.Sin(latitude1);
            double sinLat2 = Math.Sin(latitude2);

            double cosLat1 = Math.Cos(latitude1);
            double cosLat2 = Math.Cos(latitude2);

            double sinDeltaLon = Math.Sin(deltaLong);
            double cosDeltaLon = Math.Cos(deltaLong);


            double part1 = Math.Pow(cosLat2 * sinDeltaLon, 2.0);
            double part2 = Math.Pow(cosLat1 * sinLat2 - sinLat1 * cosLat2 * cosDeltaLon, 2.0);
            double part3 = sinLat1 * sinLat2 + cosLat1 * cosLat2 * cosDeltaLon;
            double ro = Math.Atan(Math.Sqrt(part1 + part2) / part3);

            return radious * ro;
        }
        */
        #endregion // Distance
    }

    public class VenueList : List<Venue> 
    {
        public Venue this[Guid id]
        {
            get
            {
                foreach (Venue item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }
}

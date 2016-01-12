using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class Venue
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public string Address { get; set; }

        [DataMember()]
        public string Distance { get; set; }

        internal double DistanceInYards { get; set; }

        internal Guid ChainId { get; set; }

        public Venue() { }

        public Venue(OM.Venue venue)
        {
            this.Id = venue.Id;
            this.Name = venue.Name;
            this.ChainId = venue.ChainId;
            if (venue.Address != null && !venue.Address.IsEmpty())
            {
                this.Address = venue.Address.ToShortString();
            }
        }

        internal static int CompareByDistance(Venue v1, Venue v2)
        {
            return v1.DistanceInYards.CompareTo(v2.DistanceInYards);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [DataContract()]
    public class VenueList : BaseResponse
    {
        [DataMember()]
        public List<Venue> Venues { get { return this.venues; } }
        private List<Venue> venues = new List<Venue>();

        public override string ToString()
        {
            return this.venues.Count.ToString();
        }

        internal VenueList FilterByChainId(Guid chainId)
        {
            VenueList filtered = new VenueList();
            filtered.OperationResult.ErrorCode = this.OperationResult.ErrorCode;
            filtered.OperationResult.ErrorMessage = this.OperationResult.ErrorMessage;

            foreach (Venue venue in this.venues)
            {
                if (venue.ChainId == chainId)
                {
                    filtered.venues.Add(venue);
                }
            }

            return filtered;
        }
    }
}
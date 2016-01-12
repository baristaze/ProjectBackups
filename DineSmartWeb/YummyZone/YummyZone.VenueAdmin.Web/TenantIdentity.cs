using System;

namespace YummyZone.VenueAdmin.Web
{
    public enum UserType
    {
        Customer = 0,
        SupportAgent = 1,
        SystemAdmin = 2
    }

    public class TenantIdentity
    {
        public UserType UserType { get; set; }
        public Guid GroupId { get; set; }
        public Guid ChainId { get; set; }
        public Guid VenueId { get; set; }
        public Guid UserId { get; set; }
        public string VenueName { get; set; }
        public string UserEmail { get; set; }
        public string UserFriendlyName { get; set; }

        public bool Verify(bool checkNames)
        { 
            if (this.GroupId == Guid.Empty)
            {
                return false;
            }

            if (this.ChainId == Guid.Empty)
            {
                return false;
            }

            if (this.VenueId == Guid.Empty)
            {
                return false;
            }

            if (this.UserId == Guid.Empty)
            {
                return false;
            }

            if (checkNames)
            {
                if (String.IsNullOrWhiteSpace(this.UserEmail))
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(this.VenueName))
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(this.UserFriendlyName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
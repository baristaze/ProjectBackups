using System;
using System.Data.SqlClient;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using YummyZone.ObjectModel;
using System.Collections.Generic;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class GetBranches : YummyZoneHttpHandlerJson<GetBranchesResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            if (identity.UserType != UserType.Customer)
            {
                return new GetBranchesResponse(); 
            }

            // else ... customer
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                string query = Venue.SelectAllByUserId(identity.GroupId, identity.UserId);
                VenueList venueList = Database.SelectAll<Venue, VenueList>(connection, null, query, Database.TimeoutSecs);
                VenueBranchList branches = this.GetBranchList(venueList, identity);
                GetBranchesResponse response = new GetBranchesResponse();
                response.Branches.AddRange(branches);
                return response;   
            }
        }

        private VenueBranchList GetBranchList(VenueList venueList, TenantIdentity identity)
        {
            VenueBranchList branches = new VenueBranchList();
            foreach (Venue venue in venueList)
            {
                VenueBranch branch = new VenueBranch();
                branch.Id = venue.Id;
                branch.Name = venue.Name;
                branch.Address = venue.Address.ToShortString();
                branch.IsCurrent = (venue.Id == identity.VenueId);
                branches.Add(branch);
            }

            return branches;
        }
    }

    [DataContract]
    public class VenueBranch
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public bool IsCurrent { get; set; }
    }

    public class VenueBranchList : List<VenueBranch> { }

    [DataContract]
    public class GetBranchesResponse : BaseJsonResponse
    {
        [DataMember]
        public VenueBranchList Branches { get { return this.branches; } }
        private VenueBranchList branches = new VenueBranchList();
    }
}
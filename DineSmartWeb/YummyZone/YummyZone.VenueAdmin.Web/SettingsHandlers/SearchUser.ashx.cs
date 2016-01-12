using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// SearchUser httphandler
    /// </summary>
    public class SearchUser : YummyZoneHttpHandlerJson<SearchUserResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            string searchTerm = this.GetMandatoryString(context, "q", "Query String", 300, Source.Url);
            searchTerm = "%" + searchTerm + "%"; /* includes */
            
            MemberUserList entities = new MemberUserList();
            SearchUserResponse response = new SearchUserResponse();
            
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = this.GetQuery(identity.GroupId, identity.VenueId);
                    command.Transaction = null;
                    command.CommandTimeout = Database.TimeoutSecs;
                    this.AddSqlParamForSearchTerm(command, searchTerm);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                MemberUser item = new MemberUser();
                                item.EmailAddress = reader.GetString(0);
                                entities.Add(item);
                            }
                        }
                    }
                }
            }
            
            if (entities.Count > 0)
            {
                response.Items.AddRange(entities);
            }

            return response;
        }

        private string GetQuery(Guid groupId, Guid venueId)
        {
            string query = @"SELECT [EmailAddress] FROM [dbo].[User] 
	                            WHERE [GroupId] = '{0}'
		                            AND [Status] = {2}
		                            AND [EmailAddress] LIKE @SearchTerm
		                            AND [Id] NOT IN 
		                            (
			                            SELECT DISTINCT [UserId] FROM [dbo].[UserRole]
				                            WHERE [GroupId] = '{0}'
					                            AND [VenueId] = '{1}'
		                            );";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, venueId, (int)Status.Active);
        }

        private void AddSqlParamForSearchTerm(SqlCommand command, string searchTerm)
        {
            command.Parameters.AddWithValue("@SearchTerm", searchTerm);
        }
    }

    [DataContract]
    public class SearchUserResponse : BaseJsonResponse
    {
        [DataMember]
        public MemberUserList Items { get { return this.items; } }
        private MemberUserList items = new MemberUserList();
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    /// <summary>
    /// SearchBiz httphandler
    /// </summary>
    public class SearchBiz : YummyZoneHttpHandlerJson<SearchBizResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            SystemUser user = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            string searchTerm = this.GetMandatoryString(context, "q", "Query String", 300, Source.Url);
            searchTerm = "%" + searchTerm + "%"; /* includes */

            string query = String.Empty;
            ObjectType type = ObjectType.Unspecified;
            string entityType = this.GetString(context, "t", "Entity Type", 10, Source.Url);
            if (!String.IsNullOrWhiteSpace(entityType))
            {
                if (String.Compare(entityType, "chain", StringComparison.OrdinalIgnoreCase) == 0) 
                {
                    query = SearchResult.SearchChainQuery();
                    type = ObjectType.Chain;
                }
                else if (String.Compare(entityType, "group", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    query = SearchResult.SearchGroupQuery();
                    type = ObjectType.Group;
                }
            }

            if (String.IsNullOrWhiteSpace(query))
            {
                query = SearchResult.SearchQuery();
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                                
                SearchResultList entities = new SearchResultList();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Transaction = null;
                    command.CommandTimeout = Database.TimeoutSecs;
                    SearchResult.AddSqlParameterForSearchKey(command, searchTerm);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                SearchResult item = new SearchResult();
                                item.InitFromSqlReader(reader, type);
                                entities.Add(item);
                            }
                        }
                    }
                }

                SearchBizResponse response = new SearchBizResponse();
                response.Items.AddRange(entities);

                return response;
            }
        }
    }

    [DataContract]
    public class SearchBizResponse : BaseJsonResponse
    {
        [DataMember]
        public SearchResultList Items { get { return this.items; } }
        private SearchResultList items = new SearchResultList();
    }
}
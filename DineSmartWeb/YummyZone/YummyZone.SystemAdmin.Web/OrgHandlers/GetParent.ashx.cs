using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    /// <summary>
    /// GetParent httphandler
    /// </summary>
    public class GetParent : YummyZoneHttpHandlerJson<SearchBizResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            SystemUser user = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            Guid childId = this.GetMandatoryGuid(context, "chid", "Child Id", Source.Url);
            string parentType = this.GetMandatoryString(context, "pt", "Parent Type", 10, Source.Url);

            string query = String.Empty;
            ObjectType type = ObjectType.Unspecified;

            if (String.Compare(parentType, "chain", StringComparison.OrdinalIgnoreCase) == 0)
            {
                throw new NotImplementedException();
            }
            else if (String.Compare(parentType, "group", StringComparison.OrdinalIgnoreCase) == 0)
            {
                query = SearchResult.SearchGroupByChildIdQuery(childId);
                type = ObjectType.Group;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                                
                SearchResult parent = null;
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Transaction = null;
                    command.CommandTimeout = Database.TimeoutSecs;
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                parent = new SearchResult();
                                parent.InitFromSqlReader(reader, type);
                            }
                        }
                    }
                }

                SearchBizResponse response = new SearchBizResponse();
                if (parent != null)
                {
                    response.Items.Add(parent);
                }

                return response;
            }
        }
    }
}
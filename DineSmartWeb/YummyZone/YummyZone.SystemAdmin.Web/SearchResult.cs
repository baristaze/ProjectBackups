using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public Guid ObjectId { get; set; }
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ObjectTypeAsText 
        {
            get
            {
                if (this.ObjectType == ObjectType.Venue)
                {
                    return "venue:";
                }
                else if (this.ObjectType == ObjectType.SignupVenue)
                {
                    return "venue:";
                }
                else if (this.ObjectType == ObjectType.Chain)
                {
                    return "chain:";
                }
                else if (this.ObjectType == ObjectType.Group)
                {
                    return "group:";
                }
                else
                {
                    return string.Empty;
                }                
            }
            set
            { 
            }
        }

        [DataMember]
        public string StatusAsText
        {
            get
            {
                if (this.ObjectType == ObjectModel.ObjectType.SignupVenue)
                {
                    return "(waiting)";
                }
                else
                {
                    if (this.Status == VenueStatus.Draft)
                    {
                        return "(draft)";
                    }
                    else if (this.Status == VenueStatus.Disabled || this.Status == VenueStatus.Removed)
                    {
                        return "(disabled)";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            set
            {
            }
        }

        [DataMember]
        public string AddressAsText
        {
            get
            {
                if (this.Address != null && !this.Address.IsEmpty())
                {
                    return this.Address.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
            }
        }

        [DataMember]
        public int ObjectTypeAsInt 
        { 
            get 
            {
                return (int)this.ObjectType;
            }
            set
            { 
            }
        }

        [DataMember]
        public int StatusAsInt
        {
            get
            {
                return (int)this.Status;
            }
            set
            {
            }
        }

        public ObjectType ObjectType { get; set; }
        public VenueStatus Status { get; set; }
        public string WebUrl { get; set; }
        public Address Address { get; set; }

        public static string SearchQuery() 
        {
            string query = @"(SELECT 3 AS [OrderIndex], [Id], {0} AS [ObjectType], [Status], [Name], [WebURL],
		                             NULL AS [AddressLine1], NULL AS [AddressLine2], NULL AS [City], NULL AS [State], NULL AS [ZipCode], NULL AS [Country] 
	                            FROM [dbo].[Group] 
	                            WHERE [Name] LIKE @key OR [WebURL] LIKE @key)
                             UNION
                             (SELECT 2 AS [OrderIndex], [Id], {1} AS [ObjectType], [Status], [Name], [WebURL],
		                            NULL AS [AddressLine1], NULL AS [AddressLine2], NULL AS [City], NULL AS [State], NULL AS [ZipCode], NULL AS [Country] 
	                            FROM [dbo].[Chain] 
	                            WHERE [Name] LIKE @key OR [WebURL] LIKE @key)
                             UNION
                             (SELECT 0 AS [OrderIndex], [V].[Id], {2} AS [ObjectType], [V].[Status], [V].[Name], [V].[WebURL],
		                            [A].[AddressLine1], [A].[AddressLine2], [A].[City], [A].[State], [A].[ZipCode], [A].[Country]
	                            FROM [dbo].[Venue] [V]
	                            LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {4} AND [A].[AddressType] = {6} AND [A].[ObjectId] = [V].[Id]
	                            WHERE [V].[Name] LIKE @key OR [V].[WebURL] LIKE @key OR 
		                              [A].[AddressLine1] LIKE @key OR [A].[AddressLine2] LIKE @key OR 
		                              [A].[City] LIKE @key OR [A].[State] LIKE @key OR [A].[ZipCode] LIKE @key)
                             UNION
                             (SELECT 1 AS [OrderIndex], [V].[Id], {3} AS [ObjectType], {8} AS [Status], [V].[VenueName], [V].[VenueURL],
		                            [A].[AddressLine1], [A].[AddressLine2], [A].[City], [A].[State], [A].[ZipCode], [A].[Country]
	                            FROM [dbo].[SignupInfo] [V]
	                            LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {5} AND [A].[AddressType] = {7} AND [A].[ObjectId] = [V].[Id]
	                            WHERE [V].[VenueName] LIKE @key OR [V].[VenueURL] LIKE @key OR 
		                              [A].[AddressLine1] LIKE @key OR [A].[AddressLine2] LIKE @key OR 
		                              [A].[City] LIKE @key OR [A].[State] LIKE @key OR [A].[ZipCode] LIKE @key)
                             ORDER BY [OrderIndex] ASC;";

            query = String.Format(CultureInfo.InvariantCulture, query, 
                (int)ObjectType.Group, (int)ObjectType.Chain, (int)ObjectType.Venue, (int)ObjectType.SignupVenue,
                (int)ObjectType.Venue, (int)ObjectType.SignupVenue, (int)AddressType.BusinessAddress, (int)AddressType.BusinessAddress, 
                (int)VenueStatus.Draft);

            return Database.ShortenQuery(query);
        }

        public static string SearchChainQuery()
        {
            string query = @"SELECT 0 AS [OrderIndex], [Id], {0} AS [ObjectType], [Status], [Name], [WebURL],
		                            NULL AS [AddressLine1], NULL AS [AddressLine2], NULL AS [City], NULL AS [State], NULL AS [ZipCode], NULL AS [Country] 
	                            FROM [dbo].[Chain] 
	                            WHERE [Name] LIKE @key OR [WebURL] LIKE @key";

            query = String.Format(CultureInfo.InvariantCulture, query, (int)ObjectType.Chain);
            return Database.ShortenQuery(query);
        }

        public static string SearchGroupQuery()
        {
            string query = @"SELECT 0 AS [OrderIndex], [Id], {0} AS [ObjectType], [Status], [Name], [WebURL],
		                             NULL AS [AddressLine1], NULL AS [AddressLine2], NULL AS [City], NULL AS [State], NULL AS [ZipCode], NULL AS [Country] 
	                            FROM [dbo].[Group] 
	                            WHERE [Name] LIKE @key OR [WebURL] LIKE @key";

            query = String.Format(CultureInfo.InvariantCulture, query, (int)ObjectType.Group);
            return Database.ShortenQuery(query);
        }

        public static string SearchGroupByChildIdQuery(Guid childId)
        {
            string query = @"SELECT 0 AS [OrderIndex], [G].[Id], {0} AS [ObjectType], [G].[Status], [G].[Name], [G].[WebURL],
		                             NULL AS [AddressLine1], NULL AS [AddressLine2], NULL AS [City], NULL AS [State], NULL AS [ZipCode], NULL AS [Country] 
	                            FROM [dbo].[Group] [G]
	                            JOIN [dbo].[Chain] [C] ON [G].Id = [C].[GroupId] AND [C].[Id] = '{1}'";

            query = String.Format(CultureInfo.InvariantCulture, query, (int)ObjectType.Group, childId);
            return Database.ShortenQuery(query);
        }

        public static void AddSqlParameterForSearchKey(SqlCommand command, string key)
        {
            command.Parameters.AddWithValue("@key", key);
        }

        public void InitFromSqlReader(SqlDataReader reader, ObjectType type)
        {
            // skip the order index
            int colIndex = 1;

            this.ObjectId = reader.GetGuid(colIndex++);
            this.ObjectType = (ObjectType)reader.GetInt32(colIndex++);

            if (type == ObjectType.Chain || type == ObjectType.Group)
            {
                this.Status = (VenueStatus)reader.GetByte(colIndex++);
            }
            else
            {
                this.Status = (VenueStatus)reader.GetInt32(colIndex++);
            }

            this.Name = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.WebUrl = reader.GetString(colIndex);
            }
            colIndex++; 

            this.Address = new Address();
            this.Address.InitFromSqlReader(reader, ref colIndex);
        }
    }

    public class SearchResultList : List<SearchResult> { }
}
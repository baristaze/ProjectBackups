using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{   
    public partial class SignupInfo : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            string query = @"
                    INSERT INTO [dbo].[SignupInfo]
                            ([Id]
                            ,[VenueName]
                            ,[VenueURL]
                            ,[VenueTimeZoneWinIndex]
                            ,[UserFirstName]
                            ,[UserLastName]
                            ,[UserPhoneNumber]
                            ,[UserEmailAddress]
                            ,[UserPassword]
                            ,[CreateTimeUTC])
                        VALUES
                            (@Id
                            ,@VenueName
                            ,@VenueURL
                            ,@VenueTimeZoneWinIndex
                            ,@UserFirstName
                            ,@UserLastName
                            ,@UserPhoneNumber
                            ,@UserEmailAddress
                            ,@UserPassword
                            ,@CreateTimeUTC)";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            return "DELETE FROM [dbo].[SignupInfo] WHERE [Id] = @Id;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            command.Parameters.AddWithValue("@Id", this.Id);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@VenueName", this.VenueName);
                command.Parameters.AddWithValue("@VenueURL", String.IsNullOrWhiteSpace(this.VenueURL) ? DBNull.Value : (object)this.VenueURL);
                command.Parameters.AddWithValue("@VenueTimeZoneWinIndex", this.VenueTimeZoneWinIndex.HasValue ? this.VenueTimeZoneWinIndex.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserFirstName", this.UserFirstName);
                command.Parameters.AddWithValue("@UserLastName", this.UserLastName);
                command.Parameters.AddWithValue("@UserPhoneNumber", this.UserPhoneNumber);
                command.Parameters.AddWithValue("@UserEmailAddress", this.UserEmailAddress);
                command.Parameters.AddWithValue("@UserPassword", this.UserPassword);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
            }
        }

        public static void AddSqlParameterForEmail(SqlCommand command, string email)
        {
            command.Parameters.AddWithValue("@UserEmailAddress", email);
        }

        public string SelectQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"   SELECT [V].[Id]
                                      ,[V].[VenueName]
                                      ,[V].[VenueURL]
                                      ,[V].[VenueTimeZoneWinIndex]
                                      ,[V].[UserFirstName]
                                      ,[V].[UserLastName]
                                      ,[V].[UserPhoneNumber]
                                      ,[V].[UserEmailAddress]
                                      ,[V].[UserPassword]
                                      ,[V].[CreateTimeUTC]
                                      ,[A].[AddressLine1]
                                      ,[A].[AddressLine2]
                                      ,[A].[City]
                                      ,[A].[State]
                                      ,[A].[ZipCode]
                                      ,[A].[Country]
                                  FROM [dbo].[SignupInfo] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {1} AND [A].[AddressType] = {2} AND [A].[ObjectId] = [V].[Id]
                                  WHERE [V].[Id] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id, (int)ObjectType.SignupVenue, (int)AddressType.BusinessAddress);
        }

        public static string SelectByEmailQuery()
        {
            string query = @"   SELECT [V].[Id]
                                      ,[V].[VenueName]
                                      ,[V].[VenueURL]
                                      ,[V].[VenueTimeZoneWinIndex]
                                      ,[V].[UserFirstName]
                                      ,[V].[UserLastName]
                                      ,[V].[UserPhoneNumber]
                                      ,[V].[UserEmailAddress]
                                      ,[V].[UserPassword]
                                      ,[V].[CreateTimeUTC]
                                  FROM [dbo].[SignupInfo] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                  WHERE [V].[UserEmailAddress] = @UserEmailAddress;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, (int)ObjectType.SignupVenue, (int)AddressType.BusinessAddress);
        }

        public string SelectAllQuery(Guid foo)
        {
            string query = @"   SELECT [V].[Id]
                                      ,[V].[VenueName]
                                      ,[V].[VenueURL]
                                      ,[V].[VenueTimeZoneWinIndex]
                                      ,[V].[UserFirstName]
                                      ,[V].[UserLastName]
                                      ,[V].[UserPhoneNumber]
                                      ,[V].[UserEmailAddress]
                                      ,[V].[UserPassword]
                                      ,[V].[CreateTimeUTC]
                                      ,[A].[AddressLine1]
                                      ,[A].[AddressLine2]
                                      ,[A].[City]
                                      ,[A].[State]
                                      ,[A].[ZipCode]
                                      ,[A].[Country]
                                  FROM [dbo].[SignupInfo] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, (int)ObjectType.SignupVenue, (int)AddressType.BusinessAddress);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.Id = reader.GetGuid(colIndex++);
            this.VenueName = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.VenueURL = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.VenueTimeZoneWinIndex = reader.GetByte(colIndex);
            }
            colIndex++;

            this.UserFirstName = reader.GetString(colIndex++);
            this.UserLastName = reader.GetString(colIndex++);
            this.UserPhoneNumber = reader.GetString(colIndex++);
            this.UserEmailAddress = reader.GetString(colIndex++);
            this.UserPassword = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);

            this.Address = new Address();
            this.Address.ObjectType = ObjectType.SignupVenue;
            this.Address.ObjectId = this.Id;
            this.Address.AddressType = AddressType.BusinessAddress;

            this.Address.InitFromSqlReader(reader, ref colIndex);

            if (this.Address.IsEmpty())
            {
                this.Address = null;
            }
        }
    }
}

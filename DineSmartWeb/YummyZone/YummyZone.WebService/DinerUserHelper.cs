using System;
using System.Data.SqlClient;
using System.Globalization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    public class DinerUserHelper
    {
        internal static bool PasswordDefined(SqlConnection connection, string username)
        {
            // Diner and DinerPassword are updated at once atomically. Therefore it is enough to check DinerPassword
            // DinerPassword.UserName gets updated if Diner.UserName gets changes since there is a cascading rule defined.
            string query = "SELECT [UserName] FROM [dbo].[DinerPassword] WHERE [UserName] = @userName";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = OM.Database.TimeoutSecs;
                command.Parameters.Add(new SqlParameter("@userName", username));
                string user = (string)command.ExecuteScalar();

                return !String.IsNullOrWhiteSpace(user);
            }
        }

        /*
        internal static bool UsernameOrUserIdExists(SqlConnection connection, string username)
        {
            string query = "SELECT [Id] FROM [dbo].[Diner] WHERE [Id] = @userName OR [UserName] = @userName";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = OM.Database.TimeoutSecs;
                command.Parameters.Add(new SqlParameter("@userName", username));
                object oldUserId = command.ExecuteScalar();

                return (oldUserId != null && ((Guid)oldUserId) != Guid.Empty);
            }
        }
        */
        
        internal static bool UsernameOrUserIdExists(SqlConnection connection, string username)
        {
            string query = @"   declare @count int;
                                set @count = 0;
                                IF EXISTS(SELECT [UserName] FROM [dbo].[DinerPassword] WHERE [UserName] = @userName)
                                OR EXISTS(SELECT [UserName] FROM [dbo].[Diner] WHERE CAST([Id] AS varchar(100)) = @userName OR [UserName] = @userName)
                                BEGIN
	                                set @count = 1;	
                                END
                                SELECT @count;";

            query = OM.Database.ShortenQuery(query);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = OM.Database.TimeoutSecs;
                command.Parameters.Add(new SqlParameter("@userName", username));
                object count = command.ExecuteScalar();
                return (Int32.Parse(count.ToString()) > 0);
            }
        }

        internal static bool InsertUser(SqlConnection connection, Guid userId, string username, string password)
        {
            // insert
            string query = "INSERT INTO [dbo].[Diner] ([Id], [UserName], [Status]) VALUES (@userId, @userName, 0);\r\n" +
                        "INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES (@userName, @password);";

            using (SqlTransaction trans = connection.BeginTransaction())
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Transaction = trans;
                    command.CommandTimeout = OM.Database.TimeoutSecs;
                    command.Parameters.Add(new SqlParameter("@userId", userId));
                    command.Parameters.Add(new SqlParameter("@userName", username));
                    command.Parameters.Add(new SqlParameter("@password", password));

                    Exception e = null;

                    try
                    {
                        command.ExecuteNonQuery();
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        if (e != null)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            return false;
        }

        internal static bool UpdateUser(SqlConnection connection, Guid userId, string oldUserName, string newUserName, string newPassword)
        {
            using (SqlTransaction trans = connection.BeginTransaction())
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    string query = "UPDATE [dbo].[Diner] SET [UserName] = @userName WHERE [Id] = @id AND [UserName] = @oldUserName; " +
                            "UPDATE [dbo].[DinerPassword] SET [Password] = @password WHERE [UserName] = @userName; ";

                    command.CommandText = query;
                    command.Transaction = trans;
                    command.CommandTimeout = OM.Database.TimeoutSecs;
                    command.Parameters.Add(new SqlParameter("@id", userId));
                    command.Parameters.Add(new SqlParameter("@userName", newUserName));
                    command.Parameters.Add(new SqlParameter("@oldUserName", oldUserName));
                    command.Parameters.Add(new SqlParameter("@password", newPassword));

                    Exception e = null;
                    try
                    {
                        command.ExecuteNonQuery();
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        if (e != null)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            return false;
        }

        internal static bool MergeAccounts(SqlConnection connection, Guid permanentId, Guid floatingId, string caller)
        {
            using (SqlTransaction trans = connection.BeginTransaction())
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    string query = "UPDATE [dbo].[Message] SET [ReceiverId] = '{0}' WHERE [ReceiverId] = '{1}';" +
                                    "UPDATE [dbo].[Coupon] SET [ReceiverId] = '{0}' WHERE [ReceiverId] = '{1}';" +
                                    "UPDATE [dbo].[CheckIn] SET [DinerId] = '{0}' WHERE [DinerId] = '{1}';" +
                                    "UPDATE [dbo].[NotificationClient] SET [DinerId] = '{0}' WHERE [DinerId] = '{1}';" +
                                    "DELETE FROM [dbo].[DinerSettings] WHERE [DinerId] = '{1}';" +
                                    "INSERT INTO [dbo].[Log]([App],[Type],[Operation],[ContextId],[Message]) VALUES(0, 2, '{2}', '{1}', 'Merged to {0}');";

                    query = String.Format(CultureInfo.InvariantCulture, query, permanentId, floatingId, caller);

                    command.CommandText = query;
                    command.Transaction = trans;
                    command.CommandTimeout = OM.Database.TimeoutSecs;

                    Exception e = null;
                    try
                    {
                        command.ExecuteNonQuery();
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        if (e != null)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            return false;
        }

        internal static OM.Diner GetUser(SqlConnection connection, string username, string password)
        {
            string query = "SELECT [D].[Id], [D].[Status], [P].[UserName] FROM [dbo].[DinerPassword] [P] " +
                            "JOIN [dbo].[Diner] [D] ON [D].[UserName] = [P].[UserName] " +
                            "WHERE [P].[UserName] = @userName AND [P].[Password] = @password;";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;
                command.Parameters.Add(new SqlParameter("@userName", username));
                command.Parameters.Add(new SqlParameter("@password", password));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        OM.Diner diner = new OM.Diner();
                        diner.Id = reader.GetGuid(0);
                        diner.Status = (OM.Status)reader.GetByte(1);
                        diner.UserName = reader.GetString(2);
                        return diner;
                    }
                }
            }

            return null;
        }
    }
}
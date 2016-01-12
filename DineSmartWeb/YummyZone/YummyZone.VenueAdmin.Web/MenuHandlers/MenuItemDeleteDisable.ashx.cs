using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuItemDeleteDisable httphandler
    /// </summary>
    public class MenuItemDeleteDisable : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            MenuItem menuItem = new MenuItem();
            menuItem.GroupId = identity.GroupId;
            menuItem.Id = this.GetMenuItemId(context, Source.Url);
            menuItem.LastUpdateTimeUTC = DateTime.UtcNow;

            Guid categoryId = this.GetMenuCategoryId(context, Source.Url);
            bool isDelete = this.GetDeleteOrDisableAction(context, Source.Url);

            if (isDelete)
            {
                this.Delete(menuItem, categoryId);
            }
            else
            {
                throw new NotImplementedException();
            }

            return 1; // success
        }

        private void Delete(MenuItem menuItem, Guid categoryId)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    Exception e = null;

                    try
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = menuItem.DeleteQuery(categoryId);
                            command.Transaction = trans;
                            command.CommandTimeout = Database.TimeoutSecs;
                            menuItem.AddSqlParameters(command, DBaseOperation.Delete);

                            command.ExecuteNonQuery();
                        }

                        trans.Commit();
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
                            throw e;
                        }
                    }
                }
            }
        }
    }
}
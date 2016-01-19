using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class FacebookUser : UserInfo, IDBEntity
    {
        public static List<FacebookUser> ReadAllByID(SqlConnection conn, SqlTransaction trans, string concatenatedUserIds)
        {
            List<FacebookUser> users = FacebookUser.ReadAllFromDBaseByID(conn, trans, concatenatedUserIds);
            return users;
        }
    }
}

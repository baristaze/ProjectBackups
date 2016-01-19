using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class Work : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            // get properties
            
            // employer
            long employerId = Database.GetInt64OrDefault(reader, ref index);
            string employerName = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(employerName) || employerId > 0)
            {
                // at least one property is valid
                this.Employer = new NameLongIdPair(employerId, employerName);
            }

            // position
            long positionId = Database.GetInt64OrDefault(reader, ref index);
            string positionName = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(positionName) || positionId > 0)
            {
                // at least one property is valid
                this.Position = new NameLongIdPair(positionId, positionName);
            }

            // startDate
            this.StartDate = Database.GetDateTimeOrDefault(reader, ref index);

            // end Date
            this.EndDate = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans, long facebookId, byte order)
        {
            return Database.ExecNonQuery(
               conn,
               trans,
               "[dbo].[AddToWorkHistory]",
               Database.SqlParam("@FacebookId", facebookId),
               Database.SqlParam("@Order", order),
               Database.SqlParam("@EmployerId", (this.Employer == null ? 0 : this.Employer.Id)),
               Database.SqlParam("@EmployerName", (this.Employer == null ? null : this.Employer.Name)),
               Database.SqlParam("@PositionId", (this.Position == null ? 0 : this.Position.Id)),
               Database.SqlParam("@PositionName", (this.Position == null ? null : this.Position.Name)),
               Database.SqlParam("@StartDate", this.StartDate),
               Database.SqlParam("@EndDate", this.EndDate));
        }
    }

    public partial class WorkHistory : List<Work>
    {
        public static int DeleteAllFromDBase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteWorkHistory]",
                Database.SqlParam("@FacebookId", facebookId));
        }

        public int CreateAllOnDBase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            int affectedCount = 0;
            for (int x = 0; x < this.Count; x++)
            {
                affectedCount += this[x].CreateOnDBase(conn, trans, facebookId, (byte)x);
            }

            return affectedCount;
        }

        public static WorkHistory ReadAllFromDatabase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            List<Work> all = Database.ExecSProc<Work>(
                conn,
                trans,
                "[dbo].[GetWorkHistory]",
                Database.SqlParam("@FacebookId", facebookId));

            return new WorkHistory(all);
        }

    }
}

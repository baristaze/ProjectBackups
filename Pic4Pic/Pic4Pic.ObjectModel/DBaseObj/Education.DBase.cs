using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class Education : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            // get properties
            this.Type = Database.GetStringOrDefault(reader, ref index);

            // school
            long schoolId = Database.GetInt64OrDefault(reader, ref index);
            string schoolName = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(schoolName) || schoolId > 0)
            {
                // at least one property is valid
                this.School = new NameLongIdPair(schoolId, schoolName);
            }

            // concentration
            long concentrationId = Database.GetInt64OrDefault(reader, ref index);
            string concentrationName = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(concentrationName) || concentrationId > 0)
            {
                // at least one property is valid
                this.Concentration = new NameLongIdPair(concentrationId, concentrationName);
            }

            // concentration
            long degreeId = Database.GetInt64OrDefault(reader, ref index);
            string degreeName = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(degreeName) || degreeId > 0)
            {
                // at least one property is valid
                this.Degree = new NameLongIdPair(degreeId, degreeName);
            }

            this.Year = Database.GetInt32OrDefault(reader, ref index);
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans, long facebookId, byte order)
        {
            return Database.ExecNonQuery(
               conn,
               trans,
               "[dbo].[AddToEducationHistory]",
               Database.SqlParam("@FacebookId", facebookId),
               Database.SqlParam("@Order", order),
               Database.SqlParam("@Type", this.Type),
               Database.SqlParam("@SchoolId", (this.School == null ? 0 : this.School.Id)),
               Database.SqlParam("@SchoolName", (this.School == null ? null : this.School.Name)),
               Database.SqlParam("@ConcentrationId", (this.Concentration == null ? 0 : this.Concentration.Id)),
               Database.SqlParam("@ConcentrationName", (this.Concentration == null ? null : this.Concentration.Name)),
               Database.SqlParam("@DegreeId", (this.Degree == null ? 0 : this.Degree.Id)),
               Database.SqlParam("@DegreeName", (this.Degree == null ? null : this.Degree.Name)),
               Database.SqlParam("@Year", this.Year));
        }
    }

    public partial class EducationHistory : List<Education>
    {
        public static int DeleteAllFromDBase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteEducationHistory]",
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

        public static EducationHistory ReadAllFromDatabase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            List<Education> all = Database.ExecSProc<Education>(
                conn,
                trans,
                "[dbo].[GetEducationHistory]",
                Database.SqlParam("@FacebookId", facebookId));

            return new EducationHistory(all);
        }
    }
}

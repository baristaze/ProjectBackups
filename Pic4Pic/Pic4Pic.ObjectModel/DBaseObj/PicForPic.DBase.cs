using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class PicForPic : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.Id = Database.GetGuidOrDefault(reader, ref index);
            this.UserId1 = Database.GetGuidOrDefault(reader, ref index);
            this.UserId2 = Database.GetGuidOrDefault(reader, ref index);
            this.PicId1 = Database.GetGuidOrDefault(reader, ref index);
            this.PicId2 = Database.GetGuidOrDefault(reader, ref index);
            this.RequestTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.AcceptTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        // order of users do NOT matter
        public static List<PicForPic> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId1, Guid userId2, int maxCount)
        {
            if (userId1 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (1) on Pic4Pic request");
            }

            if (userId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (2) on Pic4Pic request");
            }

            if (userId1 == userId2)
            {
                throw new Pic4PicArgumentException("Choose different User IDs for Pic4Pic requests");
            }
            
            return Database.ExecSProc<PicForPic>(
                conn,
                trans,
                "[dbo].[GetPic4PicRequest]",
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@UserId1", userId1),
                Database.SqlParam("@UserId2", userId2));
        }

        public static Dictionary<Guid, List<PicForPic>> ReadAllFromDBaseForMultipleUsers(SqlConnection conn, SqlTransaction trans, Guid userId, String concatenatedUserIDs, int maxCount)
        {
            if (userId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID to retrieve Pic4Pic requests");
            }

            if (String.IsNullOrWhiteSpace(concatenatedUserIDs))
            {
                throw new Pic4PicArgumentException("Invalid User IDs to retrieve Pic4Pic requests");
            }

            List<PicForPic> pic4pics = Database.ExecSProc<PicForPic>(
                conn,
                trans,
                "[dbo].[GetPic4PicRequests]",
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ConcatenatedUserIDs", concatenatedUserIDs));

            Dictionary<Guid, List<PicForPic>> map = new Dictionary<Guid, List<PicForPic>>();
            foreach (PicForPic p4p in pic4pics)
            {
                Guid otherUserId = p4p.UserId1;
                if (p4p.UserId1 == userId)
                {
                    otherUserId = p4p.UserId2;
                }

                if (!map.ContainsKey(otherUserId))
                {
                    map.Add(otherUserId, new List<PicForPic>());
                }

                List<PicForPic> perUser = map[otherUserId];
                perUser.Add(p4p);
            }

            return map;

        }

        public static List<PicForPic> ReadAllPendingsFromDBase(SqlConnection conn, SqlTransaction trans, Guid myUserId, int maxCount)
        {
            if (myUserId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID on Pic4Pic request");
            }

            return Database.ExecSProc<PicForPic>(
                conn,
                trans,
                "[dbo].[GetPendingReceivedPic4PicRequests]",
                Database.SqlParam("@maxCount", maxCount),
                Database.SqlParam("@UserId", myUserId));
        }

        public static Dictionary<Guid, PicForPic> ReadLastPendingsFromDBaseAndHash(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount) 
        {
            List<PicForPic> pic4pics = PicForPic.ReadAllPendingsFromDBase(conn, trans, userId, maxCount);
            Dictionary<Guid, PicForPic> mapPic4Pics = new Dictionary<Guid, PicForPic>(pic4pics.Count);
            foreach (PicForPic p4p in pic4pics)
            {
                if (mapPic4Pics.ContainsKey(p4p.UserId1))
                {
                    if (p4p.RequestTimeUTC > mapPic4Pics[p4p.UserId1].RequestTimeUTC) // only last pendings
                    {
                        mapPic4Pics[p4p.UserId1] = p4p;
                    }
                }
                else
                {
                    mapPic4Pics.Add(p4p.UserId1, p4p);
                }
            }

            return mapPic4Pics;
        }

        public static PicForPic ReadFromDBase(SqlConnection conn, SqlTransaction trans, Guid pic4picId)
        {
            if (pic4picId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Pic4Pic request ID");
            }

            List<PicForPic> items = Database.ExecSProc<PicForPic>(
                conn,
                trans,
                "[dbo].[GetPic4PicRequestById]",
                Database.SqlParam("@Id", pic4picId));

            if (items.Count > 0) 
            {
                return items[0];
            }

            return null;
        }

        public static Guid CreateRequestOnDBase(SqlConnection conn, SqlTransaction trans, PicForPic request)
        {
            return CreateRequestOnDBase(conn, trans, request, false);
        }

        public static Guid CreateRequestOnDBase(SqlConnection conn, SqlTransaction trans, PicForPic request, bool enforcePic2)
        {
            if(request == null)
            {
                throw new Pic4PicArgumentException("Invalid Pic4Pic request (null)");
            }

            if(request.UserId1 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (1) on Pic4Pic request");
            }

            if(request.UserId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (2) on Pic4Pic request");
            }

            if(request.PicId1 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Picture ID on Pic4Pic request");
            }

            if(enforcePic2 && request.PicId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Picture ID (2) on Pic4Pic request");
            }

            Guid pic4picId = Database.ExecScalar<Guid>(
                conn,
                trans,
                "[dbo].[RequestPic4Pic]",
                Database.SqlParam("@Id", request.Id),
                Database.SqlParam("@UserId1", request.UserId1),
                Database.SqlParam("@UserId2", request.UserId2),
                Database.SqlParam("@PicId1", request.PicId1),
                Database.SqlParam("@PicId2", request.PicId2));

            if (pic4picId == Guid.Empty)
            {
                throw new Pic4PicException("Unexpected error while creating Pic4Pic request on database");
            }

            return pic4picId;
        }

        public static int AcceptRequestOnDBase(SqlConnection conn, SqlTransaction trans, PicForPic request)
        {
            return AcceptRequestOnDBase(conn, trans, request, true);
        }

        public static int AcceptRequestOnDBase(SqlConnection conn, SqlTransaction trans, PicForPic request, bool enforcePic2)
        {
            if (request == null)
            {
                throw new Pic4PicArgumentException("Invalid Pic4Pic request (null)");
            }

            if (request.Id == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Pic4Pic request ID");
            }

            if (request.UserId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid User ID (2) on Pic4Pic request");
            }

            if (enforcePic2 && request.PicId2 == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Picture ID (2) on Pic4Pic request");
            }

            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[AcceptPic4Pic]",
                Database.SqlParam("@Id", request.Id),
                Database.SqlParam("@UserId2", request.UserId2),
                Database.SqlParam("@PicId2", request.PicId2));
        }
    }
}

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public partial class PicForPic : Identifiable, IDBEntity
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public Guid UserId1 { get; set; }

        [DataMember()]
        public Guid UserId2 { get; set; }

        [DataMember()]
        public Guid PicId1 { get; set; }

        [DataMember()]
        public Guid PicId2 { get; set; }

        [DataMember()]
        public DateTime RequestTimeUTC { get; set; }

        [DataMember()]
        public DateTime AcceptTimeUTC { get; set; }

        public PicForPic()
        {
            this.Id = Guid.NewGuid();
            this.RequestTimeUTC = DateTime.UtcNow;
        }

        public bool IsAccepted()
        {
            return this.AcceptTimeUTC != default(DateTime);
        }

        public bool HasBothPictures()
        {
            return this.PicId1 != Guid.Empty && this.PicId2 != Guid.Empty;
        }

        public bool IsDuplicate(PicForPic rhs, bool ignorePic2)
        {
            if (this.UserId1 != rhs.UserId1)
            {
                return false;
            }

            if (this.UserId2 != rhs.UserId2)
            {
                return false;
            }

            if (this.PicId1 != rhs.PicId1)
            {
                return false;
            }

            if (!ignorePic2) 
            {
                if (this.PicId2 != rhs.PicId2)
                {
                    return false;
                }
            }            

            return true;
        }

        public bool IsFromOtherSide(PicForPic rhs)
        {
            if (this.UserId1 != rhs.UserId2)
            {
                return false;
            }

            if (this.UserId2 != rhs.UserId1)
            {
                return false;
            }

            return true;
        }

        //----------------------------------------------------------------
        // do not create duplicate p4p within 24 hours. it is fine otherwise...
        // if (y,x:b,a) exists for (x,y:a,b) then accept it!!! NO! let it be simple and stupid! No auto-accept!
        // if (y,x:b,?) exists for (x,y:a,?) then accept it!!! NO! let it be simple and stupid! No auto-accept!
        //----------------------------------------------------------------

        // checks to see if (y,x:b,a) exists for (x,y:a,b)
        // this is good for profile matching
        public bool IsCrossMatch(PicForPic rhs, bool ignorePic2)
        {
            if (this.UserId1 != rhs.UserId2)
            {
                return false;
            }

            if (this.UserId2 != rhs.UserId1)
            {
                return false;
            }

            if (this.PicId2 != rhs.PicId1)
            {
                return false;
            }

            if (!ignorePic2)
            {
                if (this.PicId1 != rhs.PicId2)
                {
                    return false;
                }
            }

            return true;
        }

        public static Dictionary<Guid, List<PicForPic>> GroupByImageGroupId(IEnumerable<PicForPic> pic4pics)
        {
            Dictionary<Guid, List<PicForPic>> map = new Dictionary<Guid, List<PicForPic>>();
            foreach (PicForPic p4p in pic4pics)
            {
                if (p4p.PicId1 != Guid.Empty)
                {
                    if (!map.ContainsKey(p4p.PicId1))
                    {
                        map.Add(p4p.PicId1, new List<PicForPic>());
                    }

                    map[p4p.PicId1].Add(p4p);
                }

                if (p4p.PicId2 != Guid.Empty)
                {
                    if (!map.ContainsKey(p4p.PicId2))
                    {
                        map.Add(p4p.PicId2, new List<PicForPic>());
                    }

                    map[p4p.PicId2].Add(p4p);
                }                
            }

            return map;
        }

        public static Dictionary<Guid, PicForPic> GetMapOfLastPendingPic4PicsPerUser(Guid userId, Dictionary<Guid, List<PicForPic>> allPic4PicsPerUser) 
        {
            Dictionary<Guid, PicForPic> mapOfLastPendingPic4PicsPerUser = new Dictionary<Guid, PicForPic>();
            foreach (Guid candidateUserId in allPic4PicsPerUser.Keys)
            {
                List<PicForPic> perUser = allPic4PicsPerUser[candidateUserId];
                foreach (PicForPic p4p in perUser)
                {
                    if (p4p.UserId2 == userId && !p4p.IsAccepted())
                    {
                        if (mapOfLastPendingPic4PicsPerUser.ContainsKey(p4p.UserId1))
                        {
                            if (p4p.RequestTimeUTC > mapOfLastPendingPic4PicsPerUser[p4p.UserId1].RequestTimeUTC) // only last pendings
                            {
                                mapOfLastPendingPic4PicsPerUser[p4p.UserId1] = p4p;
                            }
                        }
                        else
                        {
                            mapOfLastPendingPic4PicsPerUser.Add(p4p.UserId1, p4p);
                        }
                    }
                }
            }

            return mapOfLastPendingPic4PicsPerUser;
        }
    }
}

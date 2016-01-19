using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public enum ObjectType
    {
        [EnumMember()]
        Undefined = 0,

        [EnumMember()]
        Notification = 1,

        [EnumMember()]
        Profile = 2,
    }

    [DataContract()]
    public enum MarkingType
    {
        [EnumMember()]
        Undefined = 0,

        [EnumMember()]
        Viewed = 1,

        [EnumMember()]
        Liked = 2,
    }

    [DataContract()]
    public class MarkingRequest : BaseRequest
    {
        [DataMember()]
        public MarkingType MarkingType { get; set; }

        [DataMember()]
        public ObjectType ObjectType { get; set; }

        [DataMember()]
        public Guid ObjectId { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", this.ObjectType, this.ObjectId);
        }

        public override void Validate()
        {
            base.Validate();

            // check object type
            if (this.ObjectType == ObjectType.Notification)
            {
                // check mark type
                if (this.MarkingType != MarkingType.Viewed)
                {
                    throw new Pic4PicArgumentException("Unknown marking action", "MarkingType");
                }
            }
            else if (this.ObjectType == ObjectType.Profile)
            {
                // check mark type
                if (this.MarkingType != MarkingType.Viewed && this.MarkingType != MarkingType.Liked)
                {
                    throw new Pic4PicArgumentException("Unknown marking action", "MarkingType");
                }
            }
            else
            {
                throw new Pic4PicArgumentException("Unknown object type to mark", "ObjectType");
            }

            // check object id
            if (this.ObjectId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown object ID to mark", "ObjectId");
            }
        }
    }
}
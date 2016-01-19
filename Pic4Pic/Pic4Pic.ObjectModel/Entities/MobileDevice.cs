using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum MobileOSType 
    {
        [EnumMember()]
        Unknown = 0,

        [EnumMember()]
        Android = 1,

        [EnumMember()]
        iOS = 2,

        [EnumMember()]
        Windows = 3
    }

    [DataContract()]
    public partial class MobileDevice : IDBEntity
    {
        [DataMember()]
        public Guid ClientId { get; set; }

        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public MobileOSType OSType { get; set; }
        
        [DataMember()]
        public String OSVersion { get; set; }

        [DataMember()]
        public String AppVersion { get; set; }

	    [DataMember()]
        public String SDKVersion { get; set; }
	
	    [DataMember()]
        public String DeviceType { get; set; }

        [DataMember()]
        public String PushNotifRegId { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

        [DataMember]
        public DateTime LastUpdateTimeUTC { get; set; }
    }
}

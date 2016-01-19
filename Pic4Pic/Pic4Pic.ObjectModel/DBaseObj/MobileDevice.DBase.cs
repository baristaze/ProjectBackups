using System;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class MobileDevice : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.ClientId = Database.GetGuidOrDefault(reader, ref index);
            this.UserId = Database.GetGuidOrDefault(reader, ref index);
            this.OSType = (MobileOSType)Database.GetByteOrDefault(reader, ref index);
            this.OSVersion = Database.GetStringOrDefault(reader, ref index);
            this.AppVersion = Database.GetStringOrDefault(reader, ref index);
            this.SDKVersion = Database.GetStringOrDefault(reader, ref index);
            this.DeviceType = Database.GetStringOrDefault(reader, ref index);
            this.PushNotifRegId = Database.GetStringOrDefault(reader, ref index);
            this.CreateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.LastUpdateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public int InsertOrUpdateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[AddOrUpdateMobileDevice]",
                Database.SqlParam("@ClientId", this.ClientId),
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@OSType", (byte)this.OSType),
                Database.SqlParam("@OSVersion", this.OSVersion),
                Database.SqlParam("@AppVersion", this.AppVersion),
                Database.SqlParam("@SDKVersion", this.SDKVersion),
                Database.SqlParam("@DeviceType", this.DeviceType),
                Database.SqlParam("@PushNotifRegId", this.PushNotifRegId));
        }

        public static List<MobileDevice> ReadMobileDevicesFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            // You need to check duplicate [PushNotifRegId]s...
            // No need to check duplicate [ClientId]s since they are primary keys.
            // You shouldn't check duplicate [DeviceType]s since it is valid; i.e. a person can have 2 Samsung Galaxy S4. 
            // But what if these devices are same? Well, it is up-to Google Play to deliver SAME device key if devices are same.
            List<MobileDevice> mobileDevices = Database.ExecSProc<MobileDevice>(
                conn,
                trans,
                "[dbo].[GetMobileDevices]",
                Database.SqlParam("@UserId", userId));

            List<MobileDevice> uniqueDevices = new List<MobileDevice>();
            foreach (MobileDevice mobileDevice in mobileDevices) 
            {
                bool alreadyExists = false;
                foreach (MobileDevice uniqueDevice in uniqueDevices) 
                {
                    if (mobileDevice.PushNotifRegId == uniqueDevice.PushNotifRegId) 
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if(!alreadyExists)
                {
                    uniqueDevices.Add(mobileDevice);
                }
            }

            return uniqueDevices;
        }

        public static Dictionary<Guid, Dictionary<String, MobileDevice>> ReadMobileDevicesFromDBase(SqlConnection conn, SqlTransaction trans, string concatenatedUserIDs)
        {
            // You need to check duplicate [PushNotifRegId]s...
            // No need to check duplicate [ClientId]s since they are primary keys.
            // You shouldn't check duplicate [DeviceType]s since it is valid; i.e. a person can have 2 Samsung Galaxy S4. 
            // But what if these devices are same? Well, it is up-to Google Play to deliver SAME device key if devices are same.
            List<MobileDevice> mobileDevices = Database.ExecSProc<MobileDevice>(
                conn,
                trans,
                "[dbo].[GetMobileDevicesBulk]",
                Database.SqlParam("@concatenatedUserIds", concatenatedUserIDs));

            Dictionary<Guid, Dictionary<String, MobileDevice>> uniqueDevicesPerUser = new Dictionary<Guid, Dictionary<string, MobileDevice>>();
            foreach (MobileDevice mobileDevice in mobileDevices)
            {
                if (!uniqueDevicesPerUser.ContainsKey(mobileDevice.UserId))
                {
                    uniqueDevicesPerUser.Add(mobileDevice.UserId, new Dictionary<string, MobileDevice>());
                }

                if (!uniqueDevicesPerUser[mobileDevice.UserId].ContainsKey(mobileDevice.PushNotifRegId))
                {
                    uniqueDevicesPerUser[mobileDevice.UserId].Add(mobileDevice.PushNotifRegId, mobileDevice);
                }
            }

            return uniqueDevicesPerUser;
        }
    }
}

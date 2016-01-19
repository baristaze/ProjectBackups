using System;
using System.Collections.Generic;

using Shebeke.ObjectModel;

namespace Shebeke.LogCleaner
{
    public class LogProcessorProgram
    {
        const string devConn = "DefaultEndpointsProtocol=https;AccountName=shebeketeststorage;AccountKey=iGGvSrrMbWDWJKYJ2Z3vxrsZY35c09Iw8+EJDMQAfmv9YAItrTKy4hj47OHamV4dDE0vb0323KM6l9ivD/DaiA==";
        const string proConn = "DefaultEndpointsProtocol=https;AccountName=shebekestorage;AccountKey=dUXnrKPmD9NXGWT31rQE+5xgpJBlo5u0nlHUmzcorHPMmHNG3M+UhcRrhi3goG+HthhJ3UIHu0NMk+ZzCyDsfg==";

        static void Main(string[] args)
        {
            //DateTime start = new DateTime(2000, 1, 1);
            //DateTime end = DateTime.UtcNow.AddDays(-90);

            //int deleted = WADLogsTable.Delete(proConn, 1000);
            //int deleted = WADLogsTable.Delete(devConn, start, end);
            int deleted = WADLogsTable.DeleteAll(proConn);
            //Console.WriteLine(deleted);
        }
    }
}

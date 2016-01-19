using System;
using System.Collections.Generic;

using Shebeke.ObjectModel;
using System.Text;
using System.IO;

namespace Shebeke.LogProcessing
{
    public class LogProcessorProgram
    {
        const string devConn = "DefaultEndpointsProtocol=https;AccountName=shebeketeststorage;AccountKey=iGGvSrrMbWDWJKYJ2Z3vxrsZY35c09Iw8+EJDMQAfmv9YAItrTKy4hj47OHamV4dDE0vb0323KM6l9ivD/DaiA==";
        const string proConn = "DefaultEndpointsProtocol=https;AccountName=shebekestorage;AccountKey=dUXnrKPmD9NXGWT31rQE+5xgpJBlo5u0nlHUmzcorHPMmHNG3M+UhcRrhi3goG+HthhJ3UIHu0NMk+ZzCyDsfg==";

        static void Main(string[] args)
        {
            LogProcessor.PrintErrors(proConn, 1);

            // 2013/03/21 21:12:57 PST
            
            //DateTime startTime = new DateTime(2013, 9, 4, 18, 0, 0);
            //DateTime endTime = new DateTime(2013, 9, 6, 19, 0, 0);
            //DateTime t1 = startTime.ToUniversalTime();
            //DateTime  t2 = endTime.ToUniversalTime();
            //LogProcessor.PrintAll(proConn, t1, t2);
            //LogProcessor.PrintBySearchString(proConn, t1, t2, "[User=6]", "[UserId=6]");
            //LogProcessor.PrintBySearchString(proConn, t1, t2, "1140954168");

            //LogProcessor.PrintBySearchString(proConn, startTime, endTime, "Signin - New user created: ");

            //LogProcessor.WriteSocialSharesToFiles(proConn, 3, @"C:\Users\Baris\Desktop\Metrics\SocialShare\ShebekeSocialShares");
            //LogProcessor.WritePageVisitsToFiles(proConn, 3, @"C:\Users\Baris\Desktop\Metrics\PageVisits\TopicsPageVisit.csv");
        }
    }
}
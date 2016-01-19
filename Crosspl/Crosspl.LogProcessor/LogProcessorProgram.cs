using System;
using System.Collections.Generic;

using Crosspl.ObjectModel;
using System.Text;
using System.IO;

namespace Crosspl.LogProcessing
{
    public class LogProcessorProgram
    {
        const string devConn = "DefaultEndpointsProtocol=https;AccountName=vizibuzztest1;AccountKey=7dlhxhakkZ0F0sUTLhVlNcNJl9YD8Pi2y18ilYODy7zwz+T/f4KRSGhCoiA26aa6WBRwABYAxV8BCw3eeIjBQw==";
        const string proConn = "DefaultEndpointsProtocol=https;AccountName=crossplbizspark;AccountKey=+Fn0KrZS8785ijJ0lPqh12pBMH5jxKwZ+tDbWe3PVY3RyNsPoEIHJeyaXyszBNxD3QYkX2GRaI8ydqj3raPnRw==";

        static void Main(string[] args)
        {
            LogProcessor.PrintErrors(proConn, 1);

            // 2013/03/21 21:12:57 PST
            
            DateTime startTime = new DateTime(2013, 4, 26, 1, 0, 0);
            DateTime endTime = new DateTime(2013, 4, 26, 1, 30, 0);
            DateTime t1 = startTime.ToUniversalTime();
            DateTime  t2 = endTime.ToUniversalTime();
            //LogProcessor.PrintAll(proConn, t1, t2);
            //LogProcessor.PrintBySearchString(proConn, t1, t2, "[User=6]", "[UserId=6]");
            //LogProcessor.PrintBySearchString(proConn, t1, t2, "1140954168");

            //LogProcessor.WriteSocialSharesToFiles(proConn, 3, @"C:\Users\Baris\Desktop\Metrics\SocialShare\CrossplSocialShares");
            //LogProcessor.WritePageVisitsToFiles(proConn, 3, @"C:\Users\Baris\Desktop\Metrics\PageVisits\TopicsPageVisit.csv");
        }
    }
}
using System;
using System.Collections.Generic;

using Crosspl.ObjectModel;

namespace Crosspl.LogCleaner
{
    public class LogProcessorProgram
    {
        const string devConn = "DefaultEndpointsProtocol=https;AccountName=vizibuzztest1;AccountKey=7dlhxhakkZ0F0sUTLhVlNcNJl9YD8Pi2y18ilYODy7zwz+T/f4KRSGhCoiA26aa6WBRwABYAxV8BCw3eeIjBQw==";
        const string proConn = "DefaultEndpointsProtocol=https;AccountName=crossplbizspark;AccountKey=+Fn0KrZS8785ijJ0lPqh12pBMH5jxKwZ+tDbWe3PVY3RyNsPoEIHJeyaXyszBNxD3QYkX2GRaI8ydqj3raPnRw==";

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

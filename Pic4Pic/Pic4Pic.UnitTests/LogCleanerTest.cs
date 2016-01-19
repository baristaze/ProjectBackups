using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class LogCleanerTest
    {
        const string productionConnection = "DefaultEndpointsProtocol=https;AccountName=ginger;AccountKey=ChC10hlx4G7EWz4tVwDgZKjmx56zbyy9TGDkVmUWfgS0EW4lBXbPmsNkDfte44WfDm60cYBRagG89cqFHpI8mA==";

        [TestMethod]
        [Timeout(12*60*60*1000)] // 12 hours
        public void Test_DeleteAllLogs()
        {
            int deleted = WADLogsTable.DeleteAll(productionConnection);
            Console.WriteLine(deleted);
        }
    }
}

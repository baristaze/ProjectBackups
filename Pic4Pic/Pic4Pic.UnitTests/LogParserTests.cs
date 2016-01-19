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
    public class LogParserTests
    {
        [TestMethod]
        public void Test_LogParser()
        {
            //String log = "P4P_Metric: [Level=Metric][;][elapsed=0.01][;][split=0][;][userid=2db65d39-ecc0-4baa-ba77-ace4038cb44c][;][Method=Signin][;][Class=Pic4PicSvc][;][AppType=WebSvc]";
            // String log = "P4P_Verbose: [LogTimeUTC=10/11/2014 05:26:20][;][LogId=681fea4c-a1d7-4ea2-aafa-37a59de8389a][;][Message=Downloading Image... ][;][Level=Verbose][;][ObjectType=Image][;][ObjectId=9cfdd047-6261-4b68-b8ae-f249d96c1405][;][Line=56][;][Method=doInBackground][;][Class=ImageDownloadTask][;][File=ImageDownloadTask.java][;][ClientId=1544c58a-6dc7-4eed-a7c0-7bb8ef7fff6f][;][AppVersion=1.0][;][AppType=Android]";
            //String log = "P4P_Info: [LogTimeUTC=10/11/2014 16:41:06][;][LogId=902bf945-4544-4365-aa55-724791808539][;][Level=Info][;][Success=1][;][ElapsedTimeMSec=336][;][ImageDownload=http://ginger.blob.core.windows.net/photos/img_da155a7561d34ac3967aff680591daa5.jpeg][;][Line=87][;][Method=doInBackground][;][Class=ImageDownloadTask][;][File=ImageDownloadTask.java][;][ClientId=15635d6c-e36c-45ca-a88f-0c0fb23420d9][;][AppVersion=1.0][;][AppType=Android]";
            String log = "P4P_Info: [LogTimeUTC=10/12/2014 09:30:42][;][LogId=21b95cbb-8fc2-4c54-b752-cded4969ada9][;][Message=Sending device info to the server...][;][Level=Info][;][Line=26][;][Method=doInBackground][;][Class=TrackDeviceTask][;][File=TrackDeviceTask.java][;][ClientId=aba4bf9c-a1e0-4031-a0e2-6247dd00b8ca][;][AppVersion=1.0][;][AppType=Android]";
            LogBag logBag = LogBag.Parse(log);
            Console.WriteLine(logBag.ToString());

            
                Console.WriteLine(logBag.GetGuidByTag("LogId"));
                Console.WriteLine(logBag.GetDateTimeByTag("LogTimeUTC"));
                Console.WriteLine(logBag.GetGuidByTag("ClientId"));
                Console.WriteLine(logBag.GetGuidByTag("UserId"));
                Console.WriteLine(logBag.GetIntByTag("Split", 0));
                Console.WriteLine(logBag.GetByTag("Level"));
                Console.WriteLine(logBag.GetByTag("AppType"));
                Console.WriteLine(logBag.GetByTag("AppVersion"));
                Console.WriteLine(logBag.GetByTag("File"));
                Console.WriteLine(logBag.GetByTag("Class"));
                Console.WriteLine(logBag.GetByTag("Method"));
                Console.WriteLine(logBag.GetIntByTag("Line", 0));
                Console.WriteLine(logBag.GetByTag("Message"));
                Console.WriteLine(logBag.GetByTag("Exception"));
                Console.WriteLine(logBag.GetIntByTag("ElapsedTimeMSec", 0));
                Console.WriteLine(logBag.GetByTag("Funnel"));
                Console.WriteLine(logBag.GetByTag("Page"));
                Console.WriteLine(logBag.GetByTag("Action"));
                Console.WriteLine(logBag.GetByTag("Step"));
        }
    }
}
